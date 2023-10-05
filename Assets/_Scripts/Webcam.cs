using System;
using System.Collections.Generic;
using System.Threading;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class Webcam : MonoBehaviour
{
    private readonly List<WebCamTexture> _webcam = new();
    
    [SerializeField] private List<string> WebcamName;
    [SerializeField] private List<RawImage> Image;

    [SerializeField] private int RequestedCameraWidth = 1080;
    [SerializeField] private int RequestedCameraHeight = 720;
    
    private Texture2D _snapshot; // Texture to hold the captured webcam frame
    private const int MainWebcam = 0; // Index of the main webcam (you can change this)

    private void Start()
    {
        if (GameManager.Instance.ActivateWebcams == false)
            return;
        // Initialize webcams and set up RawImages to display their feed
        foreach (string webcamName in WebcamName)
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

    private void OnDestroy()
    {
        // Stop all webcams and release resources when the script is destroyed
        foreach (WebCamTexture webCam in _webcam)
        {
            webCam.Stop();
        }
    }

    public string GetWebcamImage()
    {
        // Capture a single frame
        _snapshot.SetPixels(_webcam[MainWebcam].GetPixels());
        _snapshot.Apply();
        
        // Encode frame as JPG (you can change the format)
        byte[] bytes = _snapshot.EncodeToJPG();
        // Convert to frame to base64
        string base64 = Convert.ToBase64String(bytes);
        

        // Start a new thread for file saving to avoid blocking the main thread
        Thread saveFile = new (() =>
            {
                // Generate a timestamp for the filename
                string timestamp = DateTime.Now.ToString("MMddHHmmssfff");
                
                #if UNITY_EDITOR
                // Save the captured image to a file
                SaveFiles.SaveFile("/../SaveFiles/", "Image" + timestamp + ".jpg", bytes);
                SaveFiles.SaveFile("/../SaveFiles/", "Base64" + timestamp + ".txt", base64);
                #endif
                
            });
        saveFile.Start();

        // Return the base64-encoded image
        return base64;
    }
    
}
