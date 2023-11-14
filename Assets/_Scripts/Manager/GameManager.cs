using System;
using System.Linq;
using Data;
using Enums;
using Scriptables;
using States.Game;
using Systems;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Manager
{
    /// <summary>
    /// Manages core game logic, handling game states and level conditions.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        // Configuration to prevent the game from starting if the UserID is not set
        [Header("Prevent Game From Starting When UserID Is Missing")] [SerializeField]
        private bool PreventGameStartWithoutUserID;

        // Game states
        private GameState _gameState;
        internal readonly GamePreparingState PreparingState = new();
        internal readonly GamePlayingLevelState PlayingLevelState = new();
        internal readonly GameLevelFinishedState LevelFinishedState = new();

        [field: Header("Level Setup GameObjects")]
        [field: SerializeField] public Transform EmojiSpawnPosition { get; private set; }
        [SerializeField] private GameObject ActionArea;
        public float ActionAreaSize { get; private set; }
        
        // Current selected/playing level
        private ScriptableLevel _level;

        // Properties for accessing game data
        public Transform ActionAreaTransform => ActionArea.transform;
        public LevelStruct Level => _level.LevelStruct;
        public LevelProgress LevelProgress => PlayingLevelState.LevelProgress;
        
        // Scoring
        internal const int BaseScoreForCompletion = 50;
        internal const int ScoreMultiplier = 10;


        private void OnEnable()
        {
            // Check if a user ID is set. Generate a Unix timestamp user ID if none is set
            if (EditorUI.EditorUI.Instance.UserID == "")
            {
                Debug.LogWarning("No UserID Set");
                if (PreventGameStartWithoutUserID)
                    EditorApplication.isPlaying = false;
                else
                    EditorUI.EditorUI.Instance.UserID = LoggingSystem.GetUnixTimestamp();
            }

            // Fetching the action area size and setting the selected level
            ActionAreaSize = ActionArea.GetComponent<Renderer>().bounds.size.z;
            _level = EditorUI.EditorUI.Instance.GetSelectedLevel();

            // Switch to the initial preparing state
            SwitchState(_gameState = PreparingState);
        }

        private void OnDestroy()
        {
            // Reset the user ID once the game ends
            EditorUI.EditorUI.Instance.ResetUserID();
        }


        private void Update()
        {
            // Start Level with space bar
            if (Input.GetButtonDown("Jump"))
                OnButtonPressed(UIType.StartStopLevel);
            
            // Stop game with escape
            else if (Input.GetButtonDown("Cancel"))
                EditorApplication.ExitPlaymode();
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
        public void StopTimeScale() => StartCoroutine(MathHelper.SLerpTimeScale(1, 0, 2f));

        /// <summary>
        /// Sets a new level for the game and the EditorUI.
        /// </summary>
        /// <param name="level">The new level to set.</param>
        public void SetNewLevel(ScriptableLevel level)
        {
            _level = level;
            EditorUI.EditorUI.Instance.SetNewLevel(level);
        }
        
        /// <summary>
        /// Increments the count of spawned emojis.
        /// </summary>
        public void IncreaseSpawnedEmotesCount() => PlayingLevelState.IncreaseSpawnedEmotesCount();

        public int GetMaxScore() => PlayingLevelState.MaxScore;
    }
}