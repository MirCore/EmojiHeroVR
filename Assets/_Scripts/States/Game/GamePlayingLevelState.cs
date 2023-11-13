using System;
using System.Collections.Generic;
using Data;
using Enums;
using Manager;
using Scriptables;
using UnityEngine;
using Utilities;

namespace States.Game
{
    /// <summary>
    /// Represents the state of the game when a level is actively being played.
    /// </summary>
    public class GamePlayingLevelState : GameState
    {
        private const int BaseScoreForCompletion = 50;
        private const int ScoreMultiplier = 10;

        public LevelProgress LevelProgress;

        public override void EnterState()
        {
            LevelProgress = new LevelProgress();
            
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
            EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;
            
            // Notify the game that a new level has started.
            EventManager.InvokeLevelStarted();
        }


        public override void LeaveState()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
            EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
        }

        /// <summary>
        /// Handles the behavior when an emote enters the action area.
        /// </summary>
        /// <param name="emote">The emote that entered.</param>
        private void OnEmoteEnteredAreaCallback(EEmote emote) => LevelProgress.AddEmoteToActionArea(emote);

        /// <summary>
        /// Handles the behavior when an emote exits the action area.
        /// </summary>
        /// <param name="emote">The emote that exited.</param>
        private void OnEmoteExitedAreaCallback(EEmote emote)
        {
            if (!LevelProgress.RemoveEmoteFromActionArea(emote))
                Debug.LogWarning($"Attempted to remove an emote that wasn't in the action area: {emote}");

            LevelProgress.FinishedEmoteCount++;
            if (GameManager.Instance.CheckLevelEndConditions(LevelProgress.FinishedEmoteCount))
                GameManager.Instance.SwitchState(GameManager.Instance.LevelFinishedState);
        }

        /// <summary>
        /// Calculates and updates the level score when an emote is fulfilled.
        /// The score is calculated based on the base score for completion and a multiplier based on the time left.
        /// </summary>
        /// <param name="emote">The emote that was fulfilled.</param>
        /// <param name="score">The base score associated with the emote.</param>
        private void OnEmoteFulfilledCallback(EEmote emote, float score)
        {
            LevelProgress.FulfilledEmoteCount++;
            LevelProgress.LevelScore += BaseScoreForCompletion + (int)(score * ScoreMultiplier) * 10;
        }

        /// <summary>
        /// Handle user interface input specific to the playing state.
        /// </summary>
        /// <param name="uiType">The type of UI interaction.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unexpected UIType is received.</exception>
        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.StopLevel:
                case UIType.StartStopLevel:
                    // Transition to the preparing state when the level is stopped.
                    GameManager.Instance.SwitchState(GameManager.Instance.PreparingState);
                    break;
                case UIType.PauseLevel:
                case UIType.StartLevel:
                case UIType.Default:
                case UIType.ContinueEndScreen:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }

        /// <summary>
        /// Handles level-specific UI inputs. These are unexpected during the playing state.
        /// </summary>
        /// <param name="level">The level associated with the UI input.</param>
        public override void HandleUIInput(ScriptableLevel level)
        {
            Debug.LogWarning("Level input is not expected during the playing state.");
        }

        /// <summary>
        /// Increments the count of spawned emotes.
        /// </summary>
        public void IncreaseSpawnedEmotesCount() => LevelProgress.SpawnedEmotesCount++;
    }
}