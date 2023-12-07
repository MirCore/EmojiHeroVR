using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Systems;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

namespace Manager
{
    /// <summary>
    /// Manages the webcams, captures and processes images.
    /// </summary>
    public class WebcamManager : MonoBehaviour
    {
        // List to store references to the active webcams.
        private static WebCamTexture Webcam;
        
        // List of RenderTextures that are set up to display the webcam feeds.
        [SerializeField] private RenderTexture RenderTexture;

        // Desired width and height for the webcam capture.
        [SerializeField] private int RequestedCameraWidth = 640;
        [SerializeField] private int RequestedCameraHeight = 480;


        // A texture for processing the webcam image.
        private static Texture2D _texture;

        private static Color32[] _pixels;

        private void Start()
        {
            // Get webcam names from the EditorUI
            string mainWebcamName = EditorUI.EditorUI.Instance.GetMainWebcam();
            
            // Set up the webcam.
            Webcam = new WebCamTexture(mainWebcamName, RequestedCameraWidth, RequestedCameraHeight);
            Webcam.Play();
            
            _texture = new Texture2D(Webcam.width, Webcam.height);
            
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }

        private void OnDestroy()
        {
            // Clean up webcam and texture, and unsubscribe from events on destruction.
            Webcam.Stop();
        
            RenderTexture.Release();
            
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        private void OnLevelFinishedCallback()
        {
            EditorUIFerStats.Instance.SnapshotFPS = $"0 ({EditorUIFerStats.Instance.SnapshotFPS})";
        }

        private void Update()
        {
            // On each frame, update the RenderTextures with the latest webcam image if it has updated.
            if (Webcam.didUpdateThisFrame)
                Graphics.Blit(Webcam, RenderTexture);
        }

        /// <summary>
        /// Capture and process a snapshot from the webcam.
        /// </summary>
        public static Color32[] TakeSnapshots()
        {
            Profiler.BeginSample("GetPixels");
            _pixels = Webcam.GetPixels32();
            Profiler.EndSample();
            
            // Return the image
            return _pixels;
        }

        public static Texture2D GetImage(Color32[] snapshot)
        {
            Profiler.BeginSample("SetPixels");
            // Convert pixels to a texture
            _texture.SetPixels32(snapshot);
            _texture.Apply();
            Profiler.EndSample();
                
            // Return the image
            return _texture;
        }
    }
}