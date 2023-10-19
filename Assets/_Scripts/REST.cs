using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Enums;
using Proyecto26;
using UnityEngine;
using UnityEngine.Profiling;

public static class Rest
{
    private static RequestHelper _currentRequest;

    // ReSharper disable Unity.PerformanceAnalysis
    public static void PostBase64(string image, string timestamp, EEmote logFer)
    {
        Profiler.BeginSample("Rest");
        RequestHelper currentRequest = GetBase64RequestHelper(image);

        RestClient.Post(currentRequest)
            .Then(response =>
            {
                Dictionary<EEmote, float> result = ConvertRestResponseToDictionary(response.Text);
                FerHandler.Instance.ProcessRestResponse(result, timestamp, logFer);
            })
            .Catch(error =>
            { 
                FerHandler.Instance.ProcessRestError(error, timestamp);
                Debug.Log("REST Error: " + error.Message);
            });
        Profiler.EndSample();
    }

    private static RequestHelper GetBase64RequestHelper(string image)
    {
        //image = File.ReadAllText("Assets/TestFiles/test_image_base64.txt");
        RequestHelper currentRequest = new()
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
[SuppressMessage("ReSharper", "InconsistentNaming")]
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