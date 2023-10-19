using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Enums;
using Proyecto26;
using UnityEngine;
using UnityEngine.Profiling;

public static class Rest
{
    private static RequestHelper _currentRequest;

    // ReSharper disable Unity.PerformanceAnalysis
    public static void PostBase64(string image, LogData logData)
    {
        Profiler.BeginSample("Rest");
        RequestHelper currentRequest = GetBase64RequestHelper(image);

        RestClient.Post(currentRequest)
            .Then(response =>
            {
                EEmote maxEmote = ConvertRestResponseToDictionary(response.Text);
                logData.FerProbabilities = JsonUtility.FromJson<Probabilities>(response.Text);
                FerHandler.Instance.ProcessRestResponse(maxEmote, logData);
            })
            .Catch(error =>
            { 
                FerHandler.Instance.ProcessRestError(error, logData);
                Debug.Log("REST Error: " + error.Message);
            });
        Profiler.EndSample();
    }

    private static RequestHelper GetBase64RequestHelper(string image)
    {
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

    private static EEmote ConvertRestResponseToDictionary(string responseText)
    {
        Probabilities probabilities = JsonUtility.FromJson<Probabilities>(responseText);
        
        Dictionary<EEmote, float> result = new()
        {
            { EEmote.Anger, probabilities.anger },
            { EEmote.Disgust, probabilities.disgust },
            { EEmote.Fear, probabilities.fear },
            { EEmote.Happiness, probabilities.happiness },
            { EEmote.Neutral, probabilities.neutral },
            { EEmote.Sadness, probabilities.sadness },
            { EEmote.Surprise, probabilities.surprise }
        };
        
        return result.OrderByDescending(kv => kv.Value).First().Key;
    }
}

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Probabilities
{
    public float anger;
    public float disgust;
    public float fear;
    public float happiness;
    public float neutral;
    public float sadness;
    public float surprise;
}