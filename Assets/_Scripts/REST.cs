using System;
using System.Threading.Tasks;
using Manager;
using Proyecto26;
using UnityEngine;
using Random = UnityEngine.Random;

public class REST
{
    private const string BasePath = "https://64f08fba8a8b66ecf779da0f.mockapi.io/emojihero/";
    private RequestHelper _currentRequest;

    public void Post()
    {
        _currentRequest = new RequestHelper
        {
            Uri = BasePath + "/post",
            Body = new RestPost
            {
                Image = "image",
                Result = (Random.Range(0, 2) == 0)
            },
            EnableDebug = false
        };
        
        RestClient.Post<RestPost>(_currentRequest)
            .Then(response =>
            {
                GameManager.Instance.ProcessRestResponse(response);
            })
            .Catch(error => Debug.Log("Error: " + error.Message));
    }

    public static async void FakePost(string image, float delaySeconds)
    {
        RestPost restPost = new ()
        {
            Result = (Random.Range(0, 2) == 0)
            //Result = false
        };

        await Task.Delay((int)(delaySeconds * 1000));
        
        if (GameManager.Instance != null)
            GameManager.Instance.ProcessRestResponse(restPost);
    }
}

[Serializable]
public class RestPost
{
    public int ID;

    public string Image;
    
    public bool Result;

    public override string ToString(){
        return JsonUtility.ToJson (this, true);
    }
}