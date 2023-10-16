using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using Systems;
using UnityEngine;
using Utilities;

public class FerHandler : Singleton<FerHandler>
{
        private DateTime _sendRestImageTimestamp;
        [SerializeField] private bool ContinuousFerMode = true;
        public EEmote LastDetectedEmote { get; private set; }

        private void Start()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;

        }


        private void OnDestroy()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
        }

        private void OnLevelStartedCallback()
        {
            if (ContinuousFerMode)
                StartCoroutine(SendRestImageContinuous());
        }

        private IEnumerator SendRestImageContinuous()
        {
            while (ContinuousFerMode)
            {
                PostRestImage();
                yield return new WaitForSecondsRealtime(.1f);
            }
        }

        private static void PostRestImage()
        {
            // Get emote in ActionArea. This is passed to the LoggingSystem to ensure that only new log entries are created when an emote is present.
            // It also prevents missing EmoteInActionArea log entries
            EEmote logFer = GameManager.Instance.GetEmojiInActionArea().FirstOrDefault();
            // Timestamp for logging
            string timestamp = SaveFiles.GetUnixTimestamp();
            
            // get a webcam frame
            string image = GameManager.Instance.Webcam.GetWebcamImage(timestamp, logFer);
            
            //Debug.Log("Time since last RestImage: " + (DateTime.Now - _sendRestImageTimestamp));
            //_sendRestImageTimestamp = DateTime.Now;
            
            // send image to FER-MS API
            Rest.PostBase64(image, timestamp, logFer);
        }

        private void OnEmoteEnteredAreaCallback(EEmote emote)
        {
            SendRestImage();
        }
        
        private static void OnEmoteExitedAreaCallback(EEmote emote)
        {
            
        }

        private void SendRestImage()
        {
            if (ContinuousFerMode)
                return;
            PostRestImage();
        }

        public void ProcessRestResponse(Dictionary<EEmote, float>  response, string timestamp, EEmote logFer)
        {
            EEmote maxEmote = response.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            LastDetectedEmote = maxEmote;
            
            if (logFer != EEmote.None)
                LoggingSystem.Instance.WriteLog(maxEmote, response, timestamp, logFer);
            
            EventManager.InvokeEmotionDetected(maxEmote);
            
#if UNITY_EDITOR
            EditorUI.EditorUI.SetRestResponseData(response);
#endif
            
            if (ContinuousFerMode || !GameManager.Instance.GetEmojiInActionArea().Any())
                return;
            SendRestImage();
        }
        
        public void ProcessRestError(Exception error, string timestamp)
        {
            if (ContinuousFerMode)
                return;
            SendRestImage();
        }
}