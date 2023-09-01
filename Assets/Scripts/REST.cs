using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using Proyecto26;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class REST : MonoBehaviour
{
    private const string BasePath = "https://64f08fba8a8b66ecf779da0f.mockapi.io/emojihero/";
    private RequestHelper _currentRequest;

    private void LogMessage(string title, string message) {
#if UNITY_EDITOR
        EditorUtility.DisplayDialog (title, message, "Ok");
#else
		Debug.Log(message);
#endif
    }
    
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
                Debug.Log(response.result.ToString());
            })
            .Catch(error => LogMessage("Error", error.Message));
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