using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Manager
{
    public class HighScoreManage : MonoBehaviour
    {        
        private int _playerId = 0;
        
        public void OnEnable()
        {
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }

        private void OnDestroy()
        {
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        private void OnLevelFinishedCallback()
        {
            List<ScriptableLevel> levels =  Resources.LoadAll<ScriptableLevel>("Levels").ToList();
            LevelProgress levelProgress = GameManager.LevelProgress;


            HighScore highScore = new()
            {
                MatchedEmojis = levelProgress.GetMatchedEmotes(_playerId),
                TotalEmotes = levelProgress.GetSpawnedEmotes(),
                LevelScore = levelProgress.GetScore(_playerId),
                UserID = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            levels.FirstOrDefault(l => l.name == GameManager.Instance.Level.LevelName)?.AddHighScore(highScore);
            
        }
    }
}