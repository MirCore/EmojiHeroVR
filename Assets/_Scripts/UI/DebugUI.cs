using System;
using Manager;
using Scriptables;
using UnityEngine;
using Utilities;

namespace UI
{
    public class DebugUI : Singleton<DebugUI>
    {
        [SerializeField] private ScriptableDebugData DebugData;
        
        
        // Time of the last FER request
        private DateTime _postTime;

        private void OnEnable()
        {
            EventManager.OnEmotionDetected += EmotionDetectedCallback;
            EventManager.OnFerCall += FerCallCallback;
        }
        
        private void OnDestroy()
        {
            EventManager.OnEmotionDetected -= EmotionDetectedCallback;
            EventManager.OnFerCall -= FerCallCallback;
        }

        private void FerCallCallback()
        {
            LogNewFerCall();
        }

        private void EmotionDetectedCallback(DetectedFace face)
        {
            LogFerResult(face.Probabilities);
        }
        
        /// <summary>
        /// Called when a FER response is received. Updates the UI with the response data and decrements the active post counter.
        /// </summary>
        private void LogFerResult(Probabilities probabilities)
        {
            DebugData.Probabilities = probabilities;
            DebugData.PendingFer--;
        }
        
        /// <summary>
        /// Called when a new FER request is made. Updates the time between posts, calculates the posts per second, and increments the active and total post counters.
        /// </summary>
        private void LogNewFerCall()
        {
            TimeSpan postTime = DateTime.Now - _postTime;  // Calculate time since last POST request
            if (postTime.TotalSeconds < 1)  // If less than one second has passed since the last POST request
                DebugData.CurrentFerFPS = Math.Round(1 / postTime.TotalSeconds, 1); // Update posts per second
            _postTime = DateTime.Now;  // Update last POST request time
        
            DebugData.PendingFer++;  // Increment active POST request counter
            DebugData.TotalFer++;  // Increment total POST request counter
        }
    }
}
