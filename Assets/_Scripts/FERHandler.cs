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
    [SerializeField] private Webcam Webcam;
    
    [SerializeField] private bool PeriodicalFerMode = true;

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
        LogData logData = new()
        {
            Timestamp = SaveFiles.GetUnixTimestamp(),
            LevelID = GameManager.Instance.Level.name,
            EmoteID = GameManager.Instance.GetLevelEmojiProgress(),
            EmoteEmoji = GameManager.Instance.GetEmojiInActionArea().FirstOrDefault(),
            UserID = EditorUI.EditorUI.Instance.UserID,
            ImageTexture = new Texture2D(Webcam.Width, Webcam.Height)
        };

        StartCoroutine(GetFrameAndGenerateRestCall(logData));
    }

    private IEnumerator GetFrameAndGenerateRestCall(LogData logData)
    {
        // get a webcam frame
        Webcam.GetSnapshot(logData);
        yield return null;
        
        string image = Webcam.GetBase64(logData);
        yield return null;

        // send image to FER-MS API
        Rest.PostBase64(image, logData);
        
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

    public void ProcessRestResponse(EEmote maxEmote, LogData logData)
    {
        LoggingSystem.Instance.AddToLogDataList(logData);

        FerStats.Instance.RestResponse();
        
        EventManager.InvokeEmotionDetected(maxEmote);
        
#if UNITY_EDITOR
        EditorUI.EditorUI.SetRestResponseData(logData);
#endif
        
        if (GameManager.Instance.EmojisAreInActionArea())
            SendRestImage();
    }
    
    public void ProcessRestError(Exception error, LogData logData)
    {
        LoggingSystem.Instance.AddToLogDataList(logData);
        
        FerStats.Instance.RestResponse();
        
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