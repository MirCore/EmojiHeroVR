using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Enums;
using Systems;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

namespace Manager
{
    public class WebcamManager : Singleton<WebcamManager>
    {
        private readonly List<WebCamTexture> _webcams = new();
        [SerializeField] private List<RenderTexture> RenderTextures = new();

        [SerializeField] private int RequestedCameraWidth = 1280;
        [SerializeField] private int RequestedCameraHeight = 720;
        
        [SerializeField] private int TargetSnapshotFPS = 30;

        private Texture2D _texture;
        public int WebcamWidth => _webcams[0].width;
        public int WebcamHeight => _webcams[0].height;
        
        
        // Coroutine for continuous snapshots
        private Coroutine _coroutine;

        private void Start()
        {
            string mainWebcamName = EditorUI.EditorUI.Instance.GetMainWebcam();
            string secondaryWebcamName = EditorUI.EditorUI.Instance.GetSecondaryWebcam();
            InitializeWebcams(mainWebcamName, secondaryWebcamName);
            _texture = new Texture2D(_webcams[0].width, _webcams[0].height);
            
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            
        }

        private void OnDestroy()
        {
            // Stop all webcams and release resources when the script is destroyed
            foreach (WebCamTexture webcam in _webcams) 
                webcam.Stop();
        
            foreach (RenderTexture texture in RenderTextures) 
                texture.Release();
            
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
        }

        private void OnEmoteEnteredAreaCallback(EEmote obj)
        {
            _coroutine ??= StartCoroutine(TakeSnapshotCoroutine());
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
            if (secondaryWebcamName != "-" && secondaryWebcamName != "" && secondaryWebcamName != mainWebcamName)
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


        private IEnumerator TakeSnapshotCoroutine()
        {
            // Wait until the end of frame to ensure all events are processed and EmojisAreInActionArea is true
            yield return new WaitForEndOfFrame();
            
            // Interval between each snapshot.
            float interval = 1f / TargetSnapshotFPS;
            float firstPostTime = Time.realtimeSinceStartup;
            float nextPostTime = Time.realtimeSinceStartup + interval;

            int count = 0;
            
            while (count == 0 || GameManager.Instance.LevelProgress.EmojisAreInActionArea)
            {
                TakeSnapshots();
                count++;
                EditorUIFerStats.Instance.SnapshotFPS = Math.Round(count / (Time.realtimeSinceStartup - firstPostTime),1);

                // Calculate time needed to wait to ensure periodic execution
                float waitTime = Math.Max(nextPostTime - Time.realtimeSinceStartup, 0);
                
                if (waitTime > 0)
                    yield return new WaitForSecondsRealtime(waitTime);
                else
                {
                    yield return null;
                }

                // iterate timer to next interval
                nextPostTime += interval;
            }

            _coroutine = null;
        }

        private void TakeSnapshots()
        {
            Profiler.BeginSample("TakeSnapshots");
            
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
            
            LoggingSystem.Instance.LatestSnapshot = snapshot;

            Profiler.EndSample();
        }

        public static Snapshot GetSnapshot() => LoggingSystem.Instance.LatestSnapshot;

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