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
            string mainWebcamName = MainUI.Instance.GetMainWebcam();
            
            // Set up the webcam.
            _webcam = new WebCamTexture(mainWebcamName, RequestedCameraWidth, RequestedCameraHeight);
            _webcam.Play();
            
            _texture = new Texture2D(_webcam.width, _webcam.height);
            
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }

        private void OnDestroy()
        {
            // Clean up webcam and texture, and unsubscribe from events on destruction.
            _webcam.Stop();
        
            RenderTexture.Release();
            
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        private void OnLevelFinishedCallback()
        {
#if UNITY_EDITOR
            EditorUIFerStats.Instance.SnapshotFPS = $"0 ({EditorUIFerStats.Instance.SnapshotFPS})";
#endif
        }

        private void Update()
        {
            // On each frame, update the RenderTextures with the latest webcam image if it has updated.
            if (_webcam.didUpdateThisFrame)
                Graphics.Blit(_webcam, RenderTexture);
        }

        /// <summary>
        /// Capture and process a snapshot from the webcam.
        /// </summary>
        public static Color32[] TakeSnapshots()
        {
            _pixels = _webcam.GetPixels32();
            
            // Return the image
            return _pixels;
        }

        public static Texture2D GetImage(Color32[] snapshot)
        {
            // Convert pixels to a texture
            _texture.SetPixels32(snapshot);
            _texture.Apply();
                
            // Return the image
            return _texture;
        }
    }
}