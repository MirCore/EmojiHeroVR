using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Enums;
using Manager;
using Systems;
using UnityEngine;
using Utilities;

public class FerHandler : Singleton<FerHandler>
{
        private DateTime _sendRestImageTimestamp;
        [SerializeField] private bool PeriodicalFerMode = true;
        public EEmote LastDetectedEmote { get; private set; }
        

        private Coroutine _coroutine = null;

        private void OnEnable()
        {
            gameObject.AddComponent<FerStats>();
        }

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
                FerStats.Instance.NewPost();
                PostRestImage();
                yield return new WaitForSecondsRealtime(.15f);
            }
            
            _coroutine = null;
        }

        private void PostRestImage()
        {
            // Get emote in ActionArea. This is passed to the LoggingSystem to ensure that only new log entries are created when an emote is present.
            // It also prevents missing EmoteInActionArea log entries
            EEmote logFer = GameManager.Instance.GetEmojiInActionArea().FirstOrDefault();
            // Timestamp for logging
            string timestamp = SaveFiles.GetUnixTimestamp();

            StartCoroutine(GetFrameAndGenerateRestCall(timestamp, logFer));
        }

        private static IEnumerator GetFrameAndGenerateRestCall(string timestamp, EEmote logFer)
        {
            // get a webcam frame
            GameManager.Instance.Webcam.GetSnapshot();
            yield return null;
            
            string image = GameManager.Instance.Webcam.GetBase64(timestamp, logFer);
            yield return null;

            // send image to FER-MS API
            Rest.PostBase64(image, timestamp, logFer);
            
            yield return null;
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

            FerStats.Instance.RestResponse();
            
            EventManager.InvokeEmotionDetected(maxEmote);
            
#if UNITY_EDITOR
            EditorUI.EditorUI.SetRestResponseData(response);
#endif
            
            if (GameManager.Instance.EmojisAreInActionArea())
                SendRestImage();
        }
        
        public void ProcessRestError(Exception error, string timestamp)
        {
            FerStats.Instance.RestResponse();

            if (error.Message != "HTTP/1.1 422 Unprocessable Entity") return;
            
            if (GameManager.Instance.EmojisAreInActionArea())
                SendRestImage();
        }
}

[SuppressMessage("ReSharper", "NotAccessedField.Global")]
public class FerStats : Singleton<FerStats>
{
    private DateTime _postTime;
    [SerializeField, HideInInspector] public int CurrentActiveRestPosts;
    [SerializeField, HideInInspector] public int TotalPosts;
    [SerializeField, HideInInspector] public double CurrentTimeBetweenPosts;
    [SerializeField, HideInInspector] public double CurrentPostsFPS;
    
    internal void NewPost()
    {
        TimeSpan postTime = DateTime.Now - _postTime;
        if (postTime.TotalSeconds < 1)
        {
            CurrentTimeBetweenPosts = Math.Round(postTime.TotalMilliseconds);
            CurrentPostsFPS = Math.Round(1 / postTime.TotalSeconds, 1);
        }
        _postTime = DateTime.Now;
        CurrentActiveRestPosts++;
        TotalPosts++;
    }

    internal void RestResponse()
    {
        CurrentActiveRestPosts--;
    }

    private void NewLevel()
    {
        TotalPosts = 0;
    }

    private void OnEnable()
    {
        EventManager.OnLevelStarted += NewLevel;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStarted -= NewLevel;
    }
}