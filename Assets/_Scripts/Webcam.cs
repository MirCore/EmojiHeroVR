using System;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

public class Webcam : MonoBehaviour
{
    //private readonly List<WebCamTexture> _webcam = new();
    private WebCamTexture _mainWebcam;

    //private readonly List<string> _webcamName = new();
    private string _mainWebcamName;
    [SerializeField] private RenderTexture RenderTexture;

    [SerializeField] private int RequestedCameraWidth = 1080;
    [SerializeField] private int RequestedCameraHeight = 720;

    public int Width { get; private set; }
    public int Height { get; private set; }


    private void Start()
    {
        _mainWebcamName = EditorUI.EditorUI.Instance.GetSelectedWebcam();
        InitializeWebcam();
    }

    private void Update()
    {
        if (_mainWebcam.didUpdateThisFrame)
            Graphics.Blit(_mainWebcam, RenderTexture);
    }

    private void InitializeWebcam()
    {
        // Initialize webcam and set up RawImage to display their feed
        _mainWebcam = new WebCamTexture(_mainWebcamName, RequestedCameraWidth, RequestedCameraHeight);
        _mainWebcam.Play();
        Graphics.Blit(_mainWebcam, RenderTexture);
        /*for (int i = 0; i < Math.Min(Image.Count, _webcam.Count); i++)
        {
            Image[i].texture = _webcam[i];
            _mainWebcam[i].Play();
        }*/

        // Initialize the snapshot texture based on the main webcam
        Width = _mainWebcam.width;
        Height = _mainWebcam.height;
    }

    private void OnDestroy()
    {
        // Stop all webcams and release resources when the script is destroyed
        /*foreach (WebCamTexture webCam in _webcam)
        {
            webCam.Stop();
        }*/
        _mainWebcam.Stop();
    }

    public void GetSnapshot(LogData logData)
    {
        Profiler.BeginSample("GetPixels");
        // Capture a single frame
        logData.ImageTexture.SetPixels(_mainWebcam.GetPixels());
        logData.ImageTexture.Apply();

        Profiler.EndSample();
    }

    public static string GetBase64(LogData logData)
    {
        Profiler.BeginSample("EncodeToJPG");
        // Encode frame as JPG
        byte[] image = logData.ImageTexture.EncodeToJPG();
        Profiler.EndSample();

        Profiler.BeginSample("ToBase64String");
        // Convert to frame to base64
        string base64 = Convert.ToBase64String(image);
        Profiler.EndSample();

        // Return the base64-encoded image
        return base64;
    }
}