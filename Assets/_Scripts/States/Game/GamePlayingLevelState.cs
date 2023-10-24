using System;
using System.Collections.Generic;
using Enums;
using Manager;
using Scriptables;
using UnityEngine;

namespace States.Game
{
    /// <summary>
    /// Represents the state of the game when a level is actively being played.
    /// </summary>
    public class GamePlayingLevelState : GameState
    {
        private const int BaseScoreForCompletion = 50;
        private const int ScoreMultiplier = 10;

        /// <summary>Gets the count of finished emotes.</summary>
        public int FinishedEmoteCount { get; private set; }

        /// <summary>Gets the count of spawned emotes.</summary>
        public int SpawnedEmotesCount { get; private set; }

        /// <summary>Gets the current level score.</summary>
        public int LevelScore { get; private set; }

        /// <summary>Gets the list of emotes currently in the action area.</summary>
        public readonly List<EEmote> EmojiInActionArea = new();

        public override void EnterState()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
            EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;

            // Reset state values.
            ResetLevelState();
            
            // Notify the game that a new level has started.
            EventManager.InvokeLevelStarted();
        }


        public override void LeaveState()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
            EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;

            // Reset state values.
            ResetLevelState();
        }

        /// <summary>
        /// Resets the level state, clearing all emotes from the action area and resetting counters and scores.
        /// </summary>
        private void ResetLevelState()
        {
            FinishedEmoteCount = 0;
            LevelScore = 0;
            SpawnedEmotesCount = 0;
            EmojiInActionArea.Clear();
        }

        /// <summary>
        /// Handles the behavior when an emote enters the action area.
        /// </summary>
        /// <param name="emote">The emote that entered.</param>
        private void OnEmoteEnteredAreaCallback(EEmote emote) => EmojiInActionArea.Add(emote);

        /// <summary>
        /// Handles the behavior when an emote exits the action area.
        /// </summary>
        /// <param name="emote">The emote that exited.</param>
        private void OnEmoteExitedAreaCallback(EEmote emote)
        {
            if (!EmojiInActionArea.Remove(emote))
                Debug.LogWarning($"Attempted to remove an emote that wasn't in the action area: {emote}");

            FinishedEmoteCount++;
            if (GameManager.Instance.CheckLevelEndConditions(FinishedEmoteCount))
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
            LevelScore += BaseScoreForCompletion + (int)(score * ScoreMultiplier) * 10;
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
        public void IncreaseSpawnedEmotesCount() => SpawnedEmotesCount++;
    }
}