using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Manager;
using Proyecto26;
using UnityEngine;
using Random = UnityEngine.Random;

public class REST
{
    private const string BasePath = "http://localhost:8765/";
    private RequestHelper _currentRequest;

    public static async void FakePost(string image, float delaySeconds)
    {
        RestPost restPost = new ()
        {
            Result = (Random.Range(0, 2) == 0)
            //Result = false
        };

        await Task.Delay((int)(delaySeconds * 1000));
        
        GameManager.Instance.ProcessRestResponse(restPost);
    }
    
    public static IEnumerator FakePostCoroutine(string image, float delaySeconds)
    {
        RestPost restPost = new RestPost()
        {
            Result = (Random.Range(0, 2) == 0)
        };

        yield return new WaitForSeconds(delaySeconds);

        GameManager.Instance.ProcessRestResponse(restPost);
    }

    public static void Ping()
    {
        var currentRequest = new RequestHelper
        {
            Uri = BasePath + "ping",
            EnableDebug = false
        };
        
        RestClient.Get(currentRequest)
            .Then(response =>
            {
                Debug.Log("Response: " + response);
                //GameManager.Instance.ProcessRestResponse(response);
            })
            .Catch(error => Debug.Log("GET Error: " + error.Message));
    }

    public static void Post()
    {
        string image = System.IO.File.ReadAllText("TestFiles/test_image_base64.txt");
        var currentRequest = new RequestHelper
        {
            Uri = BasePath + "recognize/base64",
            Headers = new Dictionary<string, string>
            {
                { "accept", "application/json" }
            },
            ContentType ="text/Plain",
            Body = image,
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
}

[Serializable]
public class RestPost
{
    public int ID;

    public string Text;

    public string Image;
    
    public bool Result;

    public override string ToString(){
        return JsonUtility.ToJson (this, true);
    }
}