using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using Systems;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class FerHandler : Singleton<FerHandler>
{
        private DateTime _sendRestImageTimestamp;
        [SerializeField] private bool PeriodicalFerMode = true;
        public EEmote LastDetectedEmote { get; private set; }

        [SerializeField] private int CurrentActiveRestPosts;

        private Coroutine _coroutine = null;

        private void Start()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
        }


        private void OnDestroy()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
        }

        private IEnumerator SendRestImageContinuous()
        {
            yield return new WaitForEndOfFrame();
            
            while (PeriodicalFerMode && GameManager.Instance.EmojisAreInActionArea())
            {
                PostRestImage();
                CurrentActiveRestPosts++;
                yield return new WaitForSecondsRealtime(.15f);
            }
            
            _coroutine = null;
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

        private void SendRestImage()
        {
            if (!PeriodicalFerMode)
                PostRestImage();
            else if (_coroutine == null)
            {
                _coroutine = StartCoroutine(SendRestImageContinuous());
            }
        }

        public void ProcessRestResponse(Dictionary<EEmote, float>  response, string timestamp, EEmote logFer)
        {
            EEmote maxEmote = response.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            LastDetectedEmote = maxEmote;
            
            if (logFer != EEmote.None)
                LoggingSystem.Instance.WriteLog(maxEmote, response, timestamp, logFer);
            
            CurrentActiveRestPosts--;
            
            EventManager.InvokeEmotionDetected(maxEmote);
            
#if UNITY_EDITOR
            EditorUI.EditorUI.SetRestResponseData(response);
#endif
            
            if (PeriodicalFerMode || !GameManager.Instance.EmojisAreInActionArea())
                return;
            SendRestImage();
        }
        
        public void ProcessRestError(string timestamp)
        {
            if (PeriodicalFerMode)
                return;
            SendRestImage();
        }
}