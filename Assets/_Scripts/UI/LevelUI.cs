using System;
using System.Collections;
using Data;
using Enums;
using Manager;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text LevelName;
        [SerializeField] private TMP_Text Progress;

        private Coroutine _coroutine;
        
        private void OnEnable()
        {
            EventManager.OnEmoteExitedActionArea += EmoteExitedActionAreaCallback;
            EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;

            LoadLevelUI();
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            EventManager.OnEmoteExitedActionArea -= EmoteExitedActionAreaCallback;
            EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
        }

        /// <summary>
        /// Updates the score UI when an emote exits the action area.
        /// </summary>
        private void EmoteExitedActionAreaCallback(Emoji emoji) => UpdateUI();


        /// <summary>
        /// Updates the score UI when an emote is fulfilled.
        /// </summary>
        private void OnEmoteFulfilledCallback(Emoji emoji, TimeSpan time, int playerId) => UpdateUI();

        /// <summary>
        /// Loads the score UI with the current game progress and score.
        /// </summary>
        private void LoadLevelUI()
        {
            LevelName.text = GameManager.Instance.Level.LevelName;

            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (gameObject.activeInHierarchy)
                _coroutine ??= StartCoroutine(UpdateUICoroutine());
        }

        /// <summary>
        /// Updates the score UI based on the current game progress and score.
        /// </summary>
        private IEnumerator UpdateUICoroutine()
        {
            // Wait to the end of the frame to ensure LevelProgress has been properly updated
            yield return new WaitForEndOfFrame();
            
            LevelStruct level = GameManager.Instance.Level;
            
            LevelProgress levelProgress = GameManager.LevelProgress;

            Progress.text = level.LevelMode switch
            {
                ELevelMode.Training => "",
                ELevelMode.Predefined => $"{Math.Round((float)levelProgress.FinishedEmoteCount / level.EmoteArray.Length * 100)}%",
                _ => $"{Math.Round((float)levelProgress.FinishedEmoteCount / level.Count * 100)}%"
            };

            _coroutine = null;
        }
    }
}