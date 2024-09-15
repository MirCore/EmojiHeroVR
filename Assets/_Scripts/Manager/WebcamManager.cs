using System;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.Profiling;

namespace Manager
{
    /// <summary>
    /// Manages the webcams, captures and processes images.
    /// </summary>
    public class WebcamManager : MonoBehaviour
    {
        // List to store references to the active webcams.
        private static WebCamTexture _webcam;

        private static bool _webcamIsPlaying;
        
        // List of RenderTextures that are set up to display the webcam feeds.
        [SerializeField] private RenderTexture RenderTexture;

        // Desired width and height for the webcam capture.
        private const int RequestedCameraWidth = 640;
        private const int RequestedCameraHeight = 480;


        // A texture for processing the webcam image.
        private static Texture2D _texture;

        private static Color32[] _pixels;

        private void Start()
        {
            if (WebCamTexture.devices.Length != 0)
                SetupWebcam(WebCamTexture.devices.First());
        }

        private void OnDestroy()
        {
            // Clean up webcam and texture, and unsubscribe from events on destruction.
            _webcam.Stop();
            _webcamIsPlaying = false;
        
            RenderTexture.Release();
        }

        internal static void SetupWebcam(WebCamDevice device)
        {
            try
            {
                // Check if the current webcam is already set up.
                if (_webcam != null)
                {
                    // If there's an existing webcam and it's different from the desired one, stop it.
                    _webcam.Stop();
                    _webcam = null; // Clear the existing webcam reference.
                }

                // Set up the new webcam.
                _webcam = new WebCamTexture(device.name, RequestedCameraWidth, RequestedCameraHeight);
                _webcam.Play();
                
                if (_webcam.isPlaying)
                    _webcamIsPlaying = true;
                
                _texture = new Texture2D(_webcam.width, _webcam.height);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to setup webcam: {e}");
            }
        }

        private void Update()
        {
            // On each frame, update the RenderTextures with the latest webcam image if it has updated.
            if (_webcamIsPlaying && _webcam.didUpdateThisFrame)
                Graphics.Blit(_webcam, RenderTexture);
        }

        /// <summary>
        /// Capture and process a snapshot from the webcam.
        /// </summary>
        public static Color32[] TakeSnapshot()
        {
            Profiler.BeginSample("GetPixels32");
            
            _pixels = _webcam.GetPixels32();
            
            Profiler.EndSample();
            
            // Return the image
            return _pixels;
        }

        public static Texture2D ConvertColor32ToTexture2D(Color32[] snapshot)
        {
            Profiler.BeginSample("ConvertColor32ToTexture2D");

            if (_texture.width != _webcam.width)
                _texture = new Texture2D(_webcam.width, _webcam.height);

            // Convert pixels to a texture
            _texture.SetPixels32(snapshot);
            _texture.Apply();
            
            Profiler.EndSample();
                
            // Return the image
            return _texture;
        }

        public static float GetCameraWidth() => _webcam.width;
        public static float GetCameraHeight() => _webcam.height;

        public static float GetCameraRatio()
        {
            if (!_webcam)
                return 0;
            return (float)_webcam.width / _webcam.height;
        }

        public static bool IsCameraAvailable() => _webcamIsPlaying;
    }
}