using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enums;
using Manager;
using Systems;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Utilities;

public class Webcam : MonoBehaviour
{
    private readonly List<WebCamTexture> _webcam = new();
    
    private readonly List<string> _webcamName = new();
    [SerializeField] private List<RawImage> Image;

    [SerializeField] private int RequestedCameraWidth = 1080;
    [SerializeField] private int RequestedCameraHeight = 720;
    
    private Texture2D _snapshot; // Texture to hold the captured webcam frame
    private const int MainWebcam = 0; // Index of the main webcam (you can change this)


    private void Start()
    {
#if UNITY_EDITOR
        _webcamName.Add(EditorUI.EditorUI.Instance.GetSelectedWebcam());
        InitializeWebcam();
#else
        StartCoroutine(GetCamera());
#endif
    }

    private void InitializeWebcam()
    {
        // Initialize webcams and set up RawImages to display their feed
        foreach (string webcamName in _webcamName)
        {
            _webcam.Add(new WebCamTexture(webcamName, RequestedCameraWidth, RequestedCameraHeight));
        }

        for (int i = 0; i < Math.Min(Image.Count, _webcam.Count); i++)
        {
            Image[i].texture = _webcam[i];
            _webcam[i].Play();
        }

        // Initialize the snapshot texture based on the main webcam
        _snapshot = new Texture2D(_webcam[MainWebcam].width, _webcam[MainWebcam].height);
    }

    private IEnumerator GetCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
 
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            //WebCamDevice device = WebCamTexture.devices[0];
            _webcamName.Add(WebCamTexture.devices[0].name);
            InitializeWebcam();
        }
        else
        {
            Debug.Log("Webcam not found");
        }
    }

    private void OnDestroy()
    {
        // Stop all webcams and release resources when the script is destroyed
        foreach (WebCamTexture webCam in _webcam)
        {
            webCam.Stop();
        }
    }

    public void GetSnapshot()
    {
        Profiler.BeginSample("GetPixels");
        // Capture a single frame
        _snapshot.SetPixels(_webcam[MainWebcam].GetPixels());
        _snapshot.Apply();
        Profiler.EndSample();
    }

    public string GetBase64(string timestamp, EEmote logImage)
    {
        
        Profiler.BeginSample("EncodeToJPG");
        // Encode frame as JPG (you can change the format)
        byte[] bytes = _snapshot.EncodeToJPG();
        Profiler.EndSample();
        
        Profiler.BeginSample("ToBase64String");
        // Convert to frame to base64
        string base64 = Convert.ToBase64String(bytes);
        Profiler.EndSample();

#if UNITY_EDITOR
        if (logImage != EEmote.None)
            LoggingSystem.Instance.SetImageData(bytes, timestamp);
#endif
        
        // Return the base64-encoded image
        return base64;
    }
}
