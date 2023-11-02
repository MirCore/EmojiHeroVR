using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

public class Webcam : MonoBehaviour
{
    private readonly List<WebCamTexture> _webcams = new();
    [SerializeField] private List<RenderTexture> RenderTextures = new();

    [SerializeField] private int RequestedCameraWidth = 1080;
    [SerializeField] private int RequestedCameraHeight = 720;

    private void Start()
    {
        string mainWebcamName = EditorUI.EditorUI.Instance.GetMainWebcam();
        string secondaryWebcamName = EditorUI.EditorUI.Instance.GetSecondaryWebcam();
        InitializeWebcams(mainWebcamName, secondaryWebcamName);
    }

    private void Update()
    {
        for (int i = 0; i < _webcams.Count; i++)
            if (_webcams[i].didUpdateThisFrame)
                Blit(i);
    }

    private void InitializeWebcams(string mainWebcamName, string secondaryWebcamName)
    {
        // Initialize webcam and set up RawImage to display their feed
        _webcams.Add(new WebCamTexture(mainWebcamName, RequestedCameraWidth, RequestedCameraHeight));
        if (secondaryWebcamName != "-" && secondaryWebcamName != mainWebcamName)
            _webcams.Add(new WebCamTexture(secondaryWebcamName, RequestedCameraWidth, RequestedCameraHeight));

        for (int i = 0; i < _webcams.Count; i++)
        {
            _webcams[i].Play();
            Blit(i);
        }
    }

    private void Blit(int webcamIndex)
    {
        if (RenderTextures.Count > webcamIndex)
            Graphics.Blit(_webcams[webcamIndex], RenderTextures[webcamIndex]);
    }

    private void OnDestroy()
    {
        // Stop all webcams and release resources when the script is destroyed
        foreach (WebCamTexture webcam in _webcams) 
            webcam.Stop();
        
        foreach (RenderTexture texture in RenderTextures) 
            texture.DiscardContents();
    }

    public void GetSnapshot(LogData logData)
    {
        Profiler.BeginSample("GetPixels");
        
        // Capture a single frame
        foreach (WebCamTexture webcam in _webcams)
        {
            Texture2D texture = new(webcam.width, webcam.height);
            texture.SetPixels(webcam.GetPixels());
            texture.Apply();
            logData.ImageTextures.Add(texture);
        }

        Profiler.EndSample();
    }

    public static string GetBase64(LogData logData)
    {
        Profiler.BeginSample("EncodeToJPG");
        // Encode frame as JPG
        byte[] image = logData.ImageTextures[0].EncodeToJPG();
        Profiler.EndSample();

        Profiler.BeginSample("ToBase64String");
        // Convert to frame to base64
        string base64 = Convert.ToBase64String(image);
        Profiler.EndSample();

        // Return the base64-encoded image
        return base64;
    }
}