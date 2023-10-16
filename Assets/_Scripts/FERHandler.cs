using System;
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
        private int _restCount;
        [SerializeField] private bool ContinuousFerMode = true;

        private void Start()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
        }

        private void OnDestroy()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
        }

        private void OnEmoteEnteredAreaCallback(EEmote emote)
        {
            SendRestImage();
        }
        
        private void OnEmoteExitedAreaCallback(EEmote emote)
        {
            Debug.Log(_restCount);
        }

        private void SendRestImage()
        {
            if (!ContinuousFerMode && !GameManager.Instance.GetEmojiInActionArea().Any())
                return;
            string image = GameManager.Instance.Webcam.GetWebcamImage();
            //Debug.Log("Time since last RestImage: " + (DateTime.Now - _sendRestImageTimestamp));
            _sendRestImageTimestamp = DateTime.Now;
            Rest.PostBase64(image);
            _restCount++;
        }

        public void ProcessRestResponse(Dictionary<EEmote, float>  response)
        {
            EEmote maxEmote = response.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            
            if (!GameManager.Instance.GetEmojiInActionArea().Any())
                LoggingSystem.Instance.WriteLog(maxEmote, response);
            
            EventManager.InvokeEmotionDetected(maxEmote);
            
            SendRestImage();
            
#if UNITY_EDITOR
            EditorUI.EditorUI.SetRestResponseData(response);
#endif
        }
        
        public void ProcessRestError(Exception error)
        {
            SendRestImage();
        }
}