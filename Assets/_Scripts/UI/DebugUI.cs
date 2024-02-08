using System;
using Manager;
using Scriptables;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UI
{
    public class DebugUI : Singleton<DebugUI>
    {
        [SerializeField] private ScriptableDebugData DebugData;
        
        
        // Time of the last FER request
        private DateTime _postTime;
        private DateTime _snapshotTime;
        
        private UIDocument _uiDocument;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            
            ResetDebugData();
            
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
        }
        
        /// <summary>
        /// Called when a new FER request is made. Updates the time between posts, calculates the posts per second, and increments the active and total post counters.
        /// </summary>
        private void LogNewFerCall()
        {
            TimeSpan time = DateTime.Now - _postTime;  // Calculate time since last POST request
            if (time.TotalSeconds < 1)  // If less than one second has passed since the last POST request
                DebugData.CurrentFerFPS = Math.Round(1 / time.TotalSeconds, 1); // Update posts per second
            _postTime = DateTime.Now;  // Update last POST request time
        
            DebugData.TotalFer++;  // Increment total POST request counter
        }

        private void ResetDebugData()
        {
            DebugData.Probabilities = new Probabilities();
            DebugData.TotalFer = 0;
            DebugData.CurrentFerFPS = 0;
        }

        public void ToggleUI()
        {
            _uiDocument.enabled = !_uiDocument.enabled;
        }
    }
}
