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
        [SerializeField] private List<RenderTexture> RenderTextures = new();

        // Desired width and height for the webcam capture.
        [SerializeField] private int RequestedCameraWidth = 1280;
        [SerializeField] private int RequestedCameraHeight = 720;
        
        // The target frames per second for taking snapshots from the webcam.
        [SerializeField] private int TargetSnapshotFPS = 30;

        // A list that tracks which emotes are currently in the webcam area.
        private static readonly List<Emoji> EmojisInWebcamArea = new();

        // A texture for processing the webcam image.
        private static Texture2D _texture;

        private static Color32[] _pixels;


        // A reference to the coroutine that takes continuous snapshots.
        private Coroutine _coroutine;

        private void Start()
        {
            // Get webcam names from the EditorUI
            string mainWebcamName = EditorUI.EditorUI.Instance.GetMainWebcam();
            string secondaryWebcamName = EditorUI.EditorUI.Instance.GetSecondaryWebcam();
            
            // Set up the webcams and create a texture for image processing.
            InitializeWebcams(mainWebcamName, secondaryWebcamName);
            _texture = new Texture2D(Webcam.width, Webcam.height);
            
            EventManager.OnEmoteEnteredWebcamArea += EmoteEnteredWebcamAreaCallback;
            EventManager.OnEmoteExitedWebcamArea += EmoteExitedWebcamAreaCallback;
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }

        private void OnDestroy()
        {
            // Clean up webcam and texture, and unsubscribe from events on destruction.
            Webcam.Stop();
        
            foreach (RenderTexture texture in RenderTextures) 
                texture.Release();
            
            EventManager.OnEmoteEnteredWebcamArea -= EmoteEnteredWebcamAreaCallback;
            EventManager.OnEmoteExitedWebcamArea -= EmoteExitedWebcamAreaCallback;
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        private void OnLevelFinishedCallback()
        {
            EmojisInWebcamArea.Clear();

            StopCoroutine();

            EditorUIFerStats.Instance.SnapshotFPS = $"0 ({EditorUIFerStats.Instance.SnapshotFPS})";
        }

        private void StopCoroutine()
        {
            if (_coroutine == null)
                return;
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private void EmoteEnteredWebcamAreaCallback(Emoji emoji)
        {      
            if (!GameManager.Instance.IsPlayingLevel)
                return;
            // Add the emote to the tracking list and start the snapshot coroutine if not already running.
            EmojisInWebcamArea.Add(emoji);
            _coroutine ??= StartCoroutine(TakeSnapshotCoroutine());
        }

        private void EmoteExitedWebcamAreaCallback(Emoji emoji)
        {
            if (!GameManager.Instance.IsPlayingLevel)
                return;
            // Remove the emote from the tracking list.
            EmojisInWebcamArea.Remove(emoji);
        }

        private void Update()
        {
            // On each frame, update the RenderTextures with the latest webcam image if it has updated.
            if (Webcam.didUpdateThisFrame)
                Blit();
        }

        /// <summary>
        /// Set up each webcam and begin playing it, blit to RenderTextures to display the feed.
        /// </summary>
        private void InitializeWebcams(string mainWebcamName, string secondaryWebcamName)
        {
            Webcam = new WebCamTexture(mainWebcamName, RequestedCameraWidth, RequestedCameraHeight);

            Webcam.Play();
        }

        /// <summary>
        /// Blit the webcam feed to the corresponding RenderTexture.
        /// </summary>
        /// <param name="webcamIndex"></param>
        private void Blit()
        {          
            if (RenderTextures.Count > 0)
                Graphics.Blit(Webcam, RenderTextures[0]);
        }


        /// <summary>
        /// Coroutine to take periodic snapshots while emotes are present in the webcam area.
        /// </summary>
        /// <returns></returns>
        private IEnumerator TakeSnapshotCoroutine()
        {
            // Wait until the end of frame to ensure all events are processed and EmojisAreInActionArea is true
            yield return new WaitForEndOfFrame();
            
            // Interval between each snapshot.
            float interval = 1f / TargetSnapshotFPS;
            float firstPostTime = Time.realtimeSinceStartup;
            float nextPostTime = Time.realtimeSinceStartup + interval;

            int count = 0;
            
            while (EmojisInWebcamArea.Any())
            {
                TakeSnapshots();
                count++;
                EditorUIFerStats.Instance.SnapshotFPS = $"{Math.Round(count / (Time.realtimeSinceStartup - firstPostTime),1)}";

                // Calculate time needed to wait to ensure periodic execution
                float waitTime = Math.Max(nextPostTime - Time.realtimeSinceStartup, 0);
                
                // Wait at least a single frame, if the waitTime is 0
                if (waitTime > 0)
                    yield return new WaitForSecondsRealtime(waitTime);
                else
                    yield return null;

                // iterate timer to next interval
                nextPostTime += interval;
            }

            _coroutine = null;
        }

        /// <summary>
        /// Capture and process a snapshot from each active webcam.
        /// </summary>
        private static void TakeSnapshots()
        {
            Profiler.BeginSample("TakeSnapshots");
            
            // Capture a single frame for each webcam
            
            Profiler.BeginSample("GetPixels");
            _pixels = Webcam.GetPixels32();
            Profiler.EndSample();
            

            Profiler.EndSample();
        }

        public static Color32[] GetSnapshot() => _pixels;

        public static Texture2D GetImage(Color32[] snapshot)
        {
            Profiler.BeginSample("SetPixels");
            // Convert pixels to a texture
            _texture.SetPixels32(snapshot);
            _texture.Apply();
            Profiler.EndSample();
                
            // Return the base64-encoded image
            return _texture;
        }
    }
}