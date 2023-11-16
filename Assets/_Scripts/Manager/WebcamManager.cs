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
    public class WebcamManager : Singleton<WebcamManager>
    {
        // List to store references to the active webcams.
        private readonly List<WebCamTexture> _webcams = new();
        
        // List of RenderTextures that are set up to display the webcam feeds.
        [SerializeField] private List<RenderTexture> RenderTextures = new();

        // Desired width and height for the webcam capture.
        [SerializeField] private int RequestedCameraWidth = 1280;
        [SerializeField] private int RequestedCameraHeight = 720;
        
        // The target frames per second for taking snapshots from the webcam.
        [SerializeField] private int TargetSnapshotFPS = 30;

        // A list that tracks which emotes are currently in the webcam area.
        private readonly List<EEmote> _emotesInWebcamArea = new();

        // A texture for processing the webcam image.
        private Texture2D _texture;
        
        // Accessors for webcam width and height.
        public int WebcamWidth => _webcams[0].width;
        public int WebcamHeight => _webcams[0].height;
        
        
        // A reference to the coroutine that takes continuous snapshots.
        private Coroutine _coroutine;

        private void Start()
        {
            // Get webcam names from the EditorUI
            string mainWebcamName = EditorUI.EditorUI.Instance.GetMainWebcam();
            string secondaryWebcamName = EditorUI.EditorUI.Instance.GetSecondaryWebcam();
            
            // Set up the webcams and create a texture for image processing.
            InitializeWebcams(mainWebcamName, secondaryWebcamName);
            _texture = new Texture2D(_webcams[0].width, _webcams[0].height);
            
            EventManager.OnEmoteEnteredWebcamArea += EmoteEnteredWebcamAreaCallback;
            EventManager.OnEmoteExitedWebcamArea += EmoteExitedWebcamAreaCallback;
        }

        private void OnDestroy()
        {
            // Clean up webcams and textures, and unsubscribe from events on destruction.
            foreach (WebCamTexture webcam in _webcams) 
                webcam.Stop();
        
            foreach (RenderTexture texture in RenderTextures) 
                texture.Release();
            
            EventManager.OnEmoteEnteredWebcamArea -= EmoteEnteredWebcamAreaCallback;
            EventManager.OnEmoteExitedWebcamArea -= EmoteExitedWebcamAreaCallback;
        }

        private void EmoteEnteredWebcamAreaCallback(EEmote emote)
        {            
            // Add the emote to the tracking list and start the snapshot coroutine if not already running.
            _emotesInWebcamArea.Add(emote);
            _coroutine ??= StartCoroutine(TakeSnapshotCoroutine());
        }

        private void EmoteExitedWebcamAreaCallback(EEmote emote)
        {
            // Remove the emote from the tracking list.
            _emotesInWebcamArea.Remove(emote);
        }

        private void Update()
        {
            // On each frame, update the RenderTextures with the latest webcam image if it has updated.
            for (int i = 0; i < _webcams.Count; i++)
                if (_webcams[i].didUpdateThisFrame)
                    Blit(i);
        }

        /// <summary>
        /// Set up each webcam and begin playing it, blit to RenderTextures to display the feed.
        /// </summary>
        private void InitializeWebcams(string mainWebcamName, string secondaryWebcamName)
        {
            _webcams.Add(new WebCamTexture(mainWebcamName, RequestedCameraWidth, RequestedCameraHeight));
            
            if (secondaryWebcamName != "-" && secondaryWebcamName != "" && secondaryWebcamName != mainWebcamName)
                _webcams.Add(new WebCamTexture(secondaryWebcamName, RequestedCameraWidth, RequestedCameraHeight));

            for (int i = 0; i < _webcams.Count; i++)
            {
                _webcams[i].Play();
                Blit(i);
            }
        }

        /// <summary>
        /// Blit the webcam feed to the corresponding RenderTexture.
        /// </summary>
        /// <param name="webcamIndex"></param>
        private void Blit(int webcamIndex)
        {          
            if (RenderTextures.Count > webcamIndex)
                Graphics.Blit(_webcams[webcamIndex], RenderTextures[webcamIndex]);
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
            
            while (_emotesInWebcamArea.Any())
            {
                TakeSnapshots();
                count++;
                EditorUIFerStats.Instance.SnapshotFPS = Math.Round(count / (Time.realtimeSinceStartup - firstPostTime),1);

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
        private void TakeSnapshots()
        {
            Profiler.BeginSample("TakeSnapshots");
            
            // Initialize a new Snapshot
            Snapshot snapshot = new()
            {
                Timestamp = LoggingSystem.GetUnixTimestamp(),
                LevelID =  GameManager.Instance.Level.LevelName,
                LevelMode = GameManager.Instance.Level.LevelMode,
                EmoteEmoji = GameManager.Instance.LevelProgress.GetEmojiInActionArea,
                ImageTextures = new List<Color32[]>(),
            };
        
            // Capture a single frame for each webcam
            foreach (WebCamTexture webcam in _webcams)
            {
                Profiler.BeginSample("GetPixels");
                Color32[] pixels = webcam.GetPixels32();
                snapshot.ImageTextures.Add(pixels);
                Profiler.EndSample();
            }
            
            // Add the Snapshot to the List of Snapshots in the LoggingSystem
            LoggingSystem.Instance.LatestSnapshot = snapshot;

            Profiler.EndSample();
        }

        public static Snapshot GetSnapshot() => LoggingSystem.Instance.LatestSnapshot;

        /// <summary>
        /// Convert the snapshot to a base64 string for network transmission.
        /// </summary>
        public string GetBase64(Snapshot snapshot)
        {
            Profiler.BeginSample("SetPixels");
            // Convert pixels to a texture
            _texture.SetPixels32(snapshot.ImageTextures[0]);
            _texture.Apply();
            Profiler.EndSample();
                
            Profiler.BeginSample("EncodeToJPG");
            // Encode frame as JPG
            byte[] image = _texture.EncodeToJPG();
            Profiler.EndSample();

            Profiler.BeginSample("ToBase64String");
            // Convert to frame to base64
            string base64 = Convert.ToBase64String(image);
            Profiler.EndSample();

            // Return the base64-encoded image
            return base64;
        }
    }
}