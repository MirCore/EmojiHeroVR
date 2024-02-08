using System;
using Data;
using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EndscreenUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text MatchedEmojis;
        [SerializeField] private TMP_Text TimeBonus;
        [SerializeField] private TMP_Text TotalScore;
        
        private int _playerId;

        private void OnEnable()
        {
            LoadEndscreenUI();
        }

        private void LoadEndscreenUI()
        {
            LevelProgress levelProgress = GameManager.LevelProgress;
            int maxScore = GameManager.GetMaxScore();
        
            string maxScoreText = maxScore > 0 ? $" / {maxScore}" : "";
            TotalScore.text = $"{levelProgress.GetScore(_playerId)}{maxScoreText}";
        
            MatchedEmojis.text = $"Matched Emojis: {levelProgress.GetMatchedEmotes(_playerId)}";
            TimeBonus.text = $"Time Bonus: {Math.Round((float)(levelProgress.GetScore(_playerId) - levelProgress.GetMatchedEmotes(_playerId) * GameManager.BaseScoreForCompletion) / GameManager.ScoreMultiplier / 10, 1)}";
        }

        public void SetPlayerId(int playerId)
        {
            _playerId = playerId;
        }
    }
}
