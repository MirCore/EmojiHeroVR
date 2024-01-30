using System;
using Data;
using Enums;
using Scriptables;
using Systems;
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
        
        // Current selected/playing level
        [SerializeField] private ScriptableLevel ScriptableLevel;

        // Properties for accessing game data
        public LevelStruct Level => ScriptableLevel.LevelStruct;
        public static LevelProgress LevelProgress => LevelManager.Instance.PlayingState.LevelProgress;
        public bool LevelIsPlaying => LevelManager.Instance.LevelIsPlaying;

        private Coroutine _timescaleCoroutine;

        // Scoring
        internal const int BaseScoreForCompletion = 50;
        internal const int ScoreMultiplier = 10;

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            
            // Create an instance of the ResourceSystem
            ResourceSystem unused = new ();
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
        /// Handles button presses related to general UI interactions.
        /// </summary>
        /// <param name="uiType">Type of UI action.</param>
        public void OnButtonPressed(UIType uiType)
        { 
            switch (uiType)
            {
                case UIType.StartGame:
                    LevelManager.Instance.PrepareLevel();
                    break;
                case UIType.StartLevel:
                    LevelManager.Instance.StartLevel();
                    break;
                case UIType.ContinueEndScreen:
                    LevelManager.Instance.StopLevel();
                    break;
                case UIType.StartStopLevel:
                case UIType.StopLevel:
                case UIType.PauseLevel:
                case UIType.Default:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }

        /// <summary>
        /// Stops the game's time scale, effectively pausing in-game action.
        /// </summary>
        public void StopTimeScale()
        {
            if (_timescaleCoroutine != null)
                StopCoroutine(_timescaleCoroutine);
            _timescaleCoroutine = StartCoroutine(MathHelper.SmoothStepTimeScale(1, 0, 2f));
        }

        
        public void SetNewLevel(ScriptableLevel level)
        {
            ScriptableLevel = level;
        }


        public static int GetMaxScore() => LevelManager.Instance.PlayingState.MaxScore;

        public void RestartTimeScale()
        {
            if (_timescaleCoroutine != null)
                StopCoroutine(_timescaleCoroutine);
            _timescaleCoroutine = StartCoroutine(MathHelper.SmoothStepTimeScale(0, 1, 1f));
        }
        
        /// <summary>
        /// Sets a new level and starts the game.
        /// </summary>
        /// <param name="level">The new level to set.</param>
        public void StartGame(ScriptableLevel level)
        {
            ScriptableLevel = level;
            LevelManager.Instance.PrepareLevel();
        }
    }
}