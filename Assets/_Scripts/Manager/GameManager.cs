using Data;
using Scriptables;
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
        
        // Current selected/playing level
        [SerializeField] private ScriptableLevel ScriptableLevel;

        // Properties for accessing game data
        public LevelStruct Level => ScriptableLevel.LevelStruct;
        public static LevelProgress LevelProgress => LevelManager.Instance.PlayingState.LevelProgress;
        public static bool LevelIsPlaying => LevelManager.Instance.LevelIsPlaying;
        
        [SerializeField] public int PlayerCount = 1;

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
            //if (Input.GetButtonDown("Jump"))
            //    OnButtonPressed(UIType.StartStopLevel);
            
            // Stop game with escape
            //else if (Input.GetButtonDown("Cancel"))
            //    EditorApplication.ExitPlaymode();
            
            // Open the DebugUI
            if (Input.GetButtonDown("Debug"))
                DebugUI.Instance.ToggleUI();
        }

        /// <summary>
        /// Stops the game's timescale, effectively pausing in-game action.
        /// </summary>
        public void StopTimeScale()
        {
            if (_timescaleCoroutine != null)
                StopCoroutine(_timescaleCoroutine);
            _timescaleCoroutine = StartCoroutine(MathHelper.SmoothStepTimeScale(1, 0, 2f));
        }

        public int GetMaxScore() => LevelManager.Instance.PlayingState.MaxScore;

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

        public void OnFrontButtonPressed()
        {
            if (!LevelIsPlaying)
                LevelManager.Instance.StartLevel();
            else
                LevelManager.Instance.StopLevel();
            // TODO: Pause Level
        }
    }
}