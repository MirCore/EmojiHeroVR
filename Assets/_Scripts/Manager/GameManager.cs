using System;
using Data;
using Enums;
using Scriptables;
using States.Game;
using Systems;
using UI;
using UnityEngine;
using Utilities;

namespace Manager
{
    /// <summary>
    /// Manages core game logic, handling game states and level conditions.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        // Whether German emote names should be used
        [field: SerializeField] public bool UseGermanEmoteNames { get; private set; }

        // Game states
        private GameState _gameState;
        internal readonly GamePreparingState PreparingState = new();
        internal readonly GamePlayingLevelState PlayingLevelState = new();
        internal readonly GameLevelFinishedState LevelFinishedState = new();

        
        // Current selected/playing level
        internal ScriptableLevel ScriptableLevel;

        // Properties for accessing game data
        public LevelStruct Level => ScriptableLevel.LevelStruct;
        public LevelProgress LevelProgress => PlayingLevelState.LevelProgress;
        private bool IsPlayingLevel => _gameState == PlayingLevelState;

        private Coroutine _timescaleCoroutine;

        // Scoring
        internal const int BaseScoreForCompletion = 50;
        internal const int ScoreMultiplier = 10;

        private void OnEnable()
        {
            // Create an instance of the ResourceSystem
            ResourceSystem unused = new ();

            // Switch to the initial preparing state
            SwitchState(_gameState = PreparingState);
        }

        private void Update()
        {
            // Start Level with space bar
            if (Input.GetButtonDown("Jump"))
                OnButtonPressed(UIType.StartStopLevel);
            
            // Stop game with escape
            //else if (Input.GetButtonDown("Cancel"))
            //    EditorApplication.ExitPlaymode();
        }

        /// <summary>
        /// Switches the game to the provided state.
        /// </summary>
        /// <param name="state">The game state to switch to.</param>
        public void SwitchState(GameState state)
        {
            _gameState.LeaveState();
            _gameState = state;
            _gameState.EnterState();
        }

        /// <summary>
        /// Checks if the current level's end conditions have been met.
        /// </summary>
        /// <param name="count">The current count of spawned or processed emojis.</param>
        /// <returns>True if the end conditions are met, false otherwise.</returns>
        public bool CheckLevelEndConditions(int count)
        {
            switch (Level.LevelMode)
            {
                case ELevelMode.Count:
                    if (count >= Level.Count) // Check if Emoji count is reached
                        return true;
                    break;
                case ELevelMode.Predefined:
                    if (count >= Level.EmoteArray.Length) // Check if all predefined Emojis have been spawned
                        return true;
                    break;
                case ELevelMode.Training: // TODO: implement training end conditions
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        /// <summary>
        /// Handles button presses related to general UI interactions.
        /// </summary>
        /// <param name="uiType">Type of UI action.</param>
        public void OnButtonPressed(UIType uiType) => _gameState.HandleUIInput(uiType);

        /// <summary>
        /// Stops the game's time scale, effectively pausing in-game action.
        /// </summary>
        public void StopTimeScale()
        {
            if (_timescaleCoroutine != null)
                StopCoroutine(_timescaleCoroutine);
            _timescaleCoroutine = StartCoroutine(MathHelper.SLerpTimeScale(1, 0, 2f));
        }

        /// <summary>
        /// Sets a new level for the game and the EditorUI.
        /// </summary>
        /// <param name="level">The new level to set.</param>
        public void SetNewLevel(ScriptableLevel level)
        {
            if (!IsPlayingLevel)
                ScriptableLevel = level;
            MainUI.Instance.SetNewLevel(ScriptableLevel);
        }

        /// <summary>
        /// Increments the count of spawned emojis.
        /// </summary>
        /// <param name="emoji"></param>
        public void IncreaseSpawnedEmotesCount(Emoji emoji) => PlayingLevelState.IncreaseSpawnedEmotesCount(emoji);

        public int GetMaxScore() => PlayingLevelState.MaxScore;

        public void RestartTimeScale()
        {
            if (_timescaleCoroutine != null)
                StopCoroutine(_timescaleCoroutine);
            _timescaleCoroutine = StartCoroutine(MathHelper.SLerpTimeScale(0, 1, 1f));
        }
    }
}