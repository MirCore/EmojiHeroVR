using System;
using Data;
using Enums;
using Manager;
using UnityEngine;
using Utilities;

namespace States.Level
{
    /// <summary>
    /// Represents the state of the level when a level is actively being played.
    /// </summary>
    public class LevelPlayingState : LevelState
    {
        public LevelProgress LevelProgress;
        
        public int MaxScore { get; private set; }
        
        public override void EnterState()
        {
            LevelProgress = new LevelProgress();

            CalculateMaxScore();
            
            SubscribeToEvents(true);
            
            // Notify the game that a new level has started.
            EventManager.InvokeLevelStarted();
        }

        public override void LeaveState()
        {
            LevelProgress?.ClearEmotesInActionAreaList();
            
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool subscribe)
        {
            if (subscribe)
            {
                EventManager.OnEmoteSpawned += EmoteSpawnedCallback;
                EventManager.OnEmoteEnteredActionArea += EmoteEnteredActionAreaCallback;
                EventManager.OnEmoteExitedActionArea += EmoteExitedActionAreaCallback;
                EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;
            }
            else
            {
                EventManager.OnEmoteSpawned -= EmoteSpawnedCallback;
                EventManager.OnEmoteEnteredActionArea -= EmoteEnteredActionAreaCallback;
                EventManager.OnEmoteExitedActionArea -= EmoteExitedActionAreaCallback;
                EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
            }
        }

        private void EmoteSpawnedCallback(Emoji emoji)
        {
            IncreaseSpawnedEmotesCount(emoji);
        }

        private void CalculateMaxScore()
        {
            
            int emojiCount = 0;
            switch (GameManager.Instance.Level.LevelMode)
            {
                case ELevelMode.Predefined:
                    emojiCount = GameManager.Instance.Level.EmoteArray.Length;
                    break;
                case ELevelMode.Count:
                    emojiCount = GameManager.Instance.Level.Count;
                    break;
                case ELevelMode.Training:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MaxScore =  emojiCount * (GameManager.BaseScoreForCompletion + (2 * GameManager.ScoreMultiplier) * 10);
        }
        
        /// <summary>
        /// Handles the behavior when an emote enters the action area.
        /// </summary>
        /// <param name="emoji">The emote that entered.</param>
        private void EmoteEnteredActionAreaCallback(Emoji emoji) => LevelProgress.AddEmoteToActionArea(emoji);

        /// <summary>
        /// Handles the behavior when an emote exits the action area.
        /// </summary>
        /// <param name="emoji">The emote that exited.</param>
        private void EmoteExitedActionAreaCallback(Emoji emoji)
        {
            if (!LevelProgress.RemoveEmoteFromActionArea(emoji))
                Debug.LogWarning($"Attempted to remove an emote that wasn't in the action area: {emoji}");
            
            LevelProgress.FinishedEmotes++;
            if (LevelManager.CheckLevelEndConditions(LevelProgress.FinishedEmoteCount))
            {
                EventManager.InvokeLevelFinished();
                LevelManager.Instance.SwitchState(LevelManager.Instance.IdleState);
            }
        }

        /// <summary>
        /// Calculates and updates the level score when an emote is fulfilled.
        /// The score is calculated based on the base score for completion and a multiplier based on the time left.
        /// </summary>
        /// <param name="emoji">The emote that was fulfilled.</param>
        /// <param name="time">The base score associated with the emote.</param>
        /// <param name="playerId"></param>
        private void OnEmoteFulfilledCallback(Emoji emoji, TimeSpan time, int playerId)
        {
            int score = GameManager.BaseScoreForCompletion + (int)((2 - (float)time.Milliseconds / 1000) * GameManager.ScoreMultiplier);
            LevelProgress.OnEmoteFulfilled(score * 10, playerId);
        }

        /// <summary>
        /// Increments the count of spawned emotes.
        /// </summary>
        /// <param name="emoji"></param>
        private void IncreaseSpawnedEmotesCount(Emoji emoji) => LevelProgress.SpawnedEmotes.Add(emoji);
    }
}