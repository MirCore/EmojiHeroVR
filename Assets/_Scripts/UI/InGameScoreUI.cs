using System;
using System.Collections;
using Data;
using Enums;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class InGameScoreUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text EmojiCount;
        [SerializeField] private TMP_Text Score;
        [SerializeField] private HVLayoutGroup HVLayoutGroup;
        private int _playerId;

        private Coroutine _coroutine;

        private void OnEnable()
        {
            EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;

            // Load the UI elements for level selection.
            LoadUI();
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
        }

        /// <summary>
        /// Updates the score UI when an emote is fulfilled.
        /// </summary>
        private void OnEmoteFulfilledCallback(Emoji emoji, TimeSpan time, int playerId) => UpdateUI();

        private void LoadUI()
        {
            EmojiCount.text = "";
            Score.text = "";

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
            int maxScore = GameManager.Instance.GetMaxScore();

            LevelProgress levelProgress = GameManager.LevelProgress;

            int matchedEmotes = levelProgress.GetMatchedEmotes(_playerId);

            EmojiCount.text = level.LevelMode switch
            {
                ELevelMode.Training => $"{matchedEmotes} / {GameManager.LevelProgress.SpawnedEmotesCount}",
                ELevelMode.Endless => $"{matchedEmotes} / {GameManager.LevelProgress.SpawnedEmotesCount}",
                ELevelMode.Predefined => $"{matchedEmotes} / {level.EmoteArray.Length} ({Math.Round((float)matchedEmotes / level.EmoteArray.Length * 100, 1)}%)",
                _ => $"{matchedEmotes} / {level.Count} ({Math.Round((float)matchedEmotes / level.EmoteArray.Length * 100, 1)}%)"
            };


            string maxScoreText = "";
            if (maxScore > 0)
                maxScoreText = $" / {maxScore}";
            Score.text = $"{levelProgress.GetScore(_playerId)}{maxScoreText}";

            _coroutine = null;
        }

        public void SetPlayerId(int playerId)
        {
            _playerId = playerId;
        }

        public void SetLayoutVertical()
        {
            HVLayoutGroup.SetLayout(true);
        }

        public void SetLayoutHorizontal()
        {
            HVLayoutGroup.SetLayout(false);
        }
    }
}