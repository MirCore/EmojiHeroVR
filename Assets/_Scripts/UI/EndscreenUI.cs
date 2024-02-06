using System;
using Data;
using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EndscreenUI : MonoBehaviour
    {
        
        [Header("TMP Elements")]
        [SerializeField] private TMP_Text MatchedEmojis;
        [SerializeField] private TMP_Text TimeBonus;
        [SerializeField] private TMP_Text TotalScore;
        
        private void OnEnable()
        {
            EventManager.OnLevelFinished += LevelFinishedCallback;
        }

        private void OnDestroy()
        {
            EventManager.OnLevelFinished -= LevelFinishedCallback;
        }

        private void LevelFinishedCallback() => LoadEndscreenUI();

        private void LoadEndscreenUI()
        {
            LevelProgress levelProgress = GameManager.LevelProgress;
            int maxScore = GameManager.GetMaxScore();
        
            string maxScoreText = maxScore > 0 ? $" / {maxScore}" : "";
            TotalScore.text = $"{levelProgress.GetScore(0)}{maxScoreText}"; // TODO: MultiPlayer
        
            MatchedEmojis.text = $"Matched Emojis: {levelProgress.GetMatchedEmotes(0)}";
            TimeBonus.text = $"Time Bonus: {Math.Round((float)(levelProgress.GetScore(0) - levelProgress.GetMatchedEmotes(0) * GameManager.BaseScoreForCompletion) / GameManager.ScoreMultiplier / 10, 1)}";
        }
    }
}
