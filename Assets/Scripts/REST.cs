using System;
using System.Threading.Tasks;
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
            Body = new Post
            {
                image = "image",
                result = (Random.Range(0, 2) == 0)
            },
            EnableDebug = false
        };
        
        RestClient.Post<Post>(_currentRequest)
            .Then(response =>
            {
                GameManager.Instance.ProcessRestResponse(response);
            })
            .Catch(error => Debug.Log("Error: " + error.Message));
    }

    public async Task FakePost(float delaySeconds)
    {
        Post post = new Post()
        {
            result = (Random.Range(0, 2) == 0)
        };

        await Task.Delay((int)(delaySeconds * 1000));
        
        GameManager.Instance.ProcessRestResponse(post);
    }
}

[Serializable]
public class Post
{
    public int id;

    public string image;
    
    public bool result;

    public override string ToString(){
        return UnityEngine.JsonUtility.ToJson (this, true);
    }
}