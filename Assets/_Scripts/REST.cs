using System.Collections.Generic;
using Proyecto26;
using UnityEngine.Profiling;
using Utilities;

/// <summary>
/// Provides static methods for sending REST requests.
/// </summary>
public static class Rest
{
    private static RequestHelper _currentRequest;

    // ReSharper disable Unity.PerformanceAnalysis
    public static void PostBase64(string image, LogData logData)
    {
        Profiler.BeginSample("Rest");   // Start a profiling sample named 'Rest' to measure performance
        
        
        // Create a new HTTP request for sending a base64 image
        RequestHelper currentRequest = GetBase64RequestHelper(image);
        
        // Make a POST request using the RestClient library
        RestClient.Post(currentRequest)
            .Then(response => FerHandler.Instance.ProcessRestResponse(response.Text, logData))
            .Catch(error => FerHandler.Instance.ProcessRestError(error, logData));
        
        
        Profiler.EndSample();   // End the profiling sample
    }

    /// <summary>
    /// Creates and configures a RequestHelper object for sending a base64-encoded image.
    /// </summary>
    /// <param name="image">The base64-encoded image string.</param>
    /// <returns>A configured RequestHelper object.</returns>
    private static RequestHelper GetBase64RequestHelper(string image)
    {
        // Initialize a new RequestHelper object
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
}