﻿using System;
using System.Collections;
using Data;
using Manager;
using UnityEngine;

namespace Systems
{
    public class FaceExpressionLogger : MonoBehaviour
    {
        private FaceExpressionHandler _faceExpressionHandler;
        [SerializeField] private int TargetFPS = 0;
        private Coroutine _coroutine;

        private void Start()
        {
            _faceExpressionHandler = new FaceExpressionHandler();
        }

        private void OnEnable()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }

        private void OnDestroy()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        private void OnLevelStartedCallback()
        {
            _coroutine = StartCoroutine(LogFaceExpression());
        }

        private void OnLevelFinishedCallback()
        {
            StopCoroutine(_coroutine);
        }

        private IEnumerator LogFaceExpression()
        {
            
            // Interval between each snapshot.
            float interval = 0;
            if (TargetFPS > 0)
                interval = 1f / TargetFPS;
            float nextPostTime = Time.realtimeSinceStartup + interval;
            
            while (GameManager.Instance.IsPlayingLevel)
            {
                while (!WebcamManager.EmoteIsInWebcamArea)
                {
                    nextPostTime = Time.realtimeSinceStartup + interval;
                    yield return null;
                }

                FaceExpression faceExpression = new()
                {
                    Timestamp = LoggingSystem.GetUnixTimestamp(),
                    LevelID = GameManager.Instance.Level.LevelName,
                    EmoteID = GameManager.Instance.LevelProgress.FinishedEmoteCount,
                    EmoteEmoji = WebcamManager.EmoteInWebcamArea,
                    FaceExpressionJson = _faceExpressionHandler.GetFaceExpressionsAsJson()
                };

                LoggingSystem.Instance.AddToFaceExpressionList(faceExpression);
                
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
        }
    }
}