using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Enums;
using Manager;
using Proyecto26;
using UnityEngine;

public class REST
{
    private RequestHelper _currentRequest;

    public static void Ping()
    {
        RequestHelper currentRequest = new RequestHelper
        {
            Uri = EditorUI.EditorUI.Instance.RestBasePath + "ping",
            EnableDebug = false
        };
        
        RestClient.Get(currentRequest)
            .Then(response =>
            {
                Debug.Log("Get Response: " + response);
            })
            .Catch(error => Debug.Log("Get Error: " + error.Message));
    }

    public static async void NormalPostBase64()
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.178.33:8765/recognize/base64");

        request.Headers.Add("accept", "application/json");

        request.Content = new StringContent(File.ReadAllText("TestFiles/test_image_base64.txt").Replace("\n", string.Empty).Replace("\r", string.Empty));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Debug.Log(responseBody);
    }

    public static async void NormalPostImage()
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.178.33:8765/recognize/file");

        request.Headers.Add("accept", "application/json");


        MultipartFormDataContent content = new MultipartFormDataContent();

        content.Add(new StringContent("TestFiles/test_image.png"), "file");        
        request.Content = content;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Debug.Log(responseBody);
    }

    public static void PostImage()
    {
        MultipartFormDataContent content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(File.ReadAllBytes("TestFiles/test_image.png")), "file", Path.GetFileName("TestFiles/test_image.png"));
        RequestHelper currentRequest = new RequestHelper
        {
            Uri = EditorUI.EditorUI.Instance.RestBasePath + "recognize/file",
            Headers = new Dictionary<string, string>
            {
                { "accept", "application/json" }
            },
            ContentType = "multipart/form-data",
            Body = content,
            EnableDebug = true
        };
        
        RestClient.Post(currentRequest)
            .Then(response =>
            {
                Debug.Log("Post Response: " + response.Text);
                //GameManager.Instance.ProcessRestResponse(response);
            })
            .Catch(error => Debug.Log("Error: " + error.Message));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void PostBase64(string image)
    {
        RequestHelper currentRequest = GetBase64RequestHelper(image);

        RestClient.Post(currentRequest)
            .Then(response =>
            {
                Dictionary<EEmote, float> result = ConvertRestResponseToDictionary(response.Text);
                GameManager.Instance.ProcessRestResponse(result);
            })
            .Catch(error => Debug.Log("Error: " + error.Message));
    }

    private static RequestHelper GetBase64RequestHelper(string image)
    {
        //image = File.ReadAllText("Assets/TestFiles/test_image_base64.txt");
        RequestHelper currentRequest = new RequestHelper
        {
            Uri = EditorUI.EditorUI.Instance.RestBasePath + "recognize/base64",
            Headers = new Dictionary<string, string>
            {
                { "accept", "application/json" }
            },
            ContentType = "text/plain",
            BodyString = image,
            EnableDebug = false
        };
        return currentRequest;
    }

    private static Dictionary<EEmote, float> ConvertRestResponseToDictionary(string responseText)
    {
        EmoteResult emoteResult = JsonUtility.FromJson<EmoteResult>(responseText);
        Dictionary<EEmote, float> result = new()
        {
            { EEmote.Anger, emoteResult.anger },
            { EEmote.Disgust, emoteResult.disgust },
            { EEmote.Fear, emoteResult.fear },
            { EEmote.Happiness, emoteResult.happiness },
            { EEmote.Neutral, emoteResult.neutral },
            { EEmote.Sadness, emoteResult.sadness },
            { EEmote.Surprise, emoteResult.surprise }
        };

        return result;
    }
}

[Serializable]
public class EmoteResult
{
    public float anger;
    public float disgust;
    public float fear;
    public float happiness;
    public float neutral;
    public float sadness;
    public float surprise;
}