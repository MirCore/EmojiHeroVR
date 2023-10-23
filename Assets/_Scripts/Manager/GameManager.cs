using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Scriptables;
using States.Game;
using Systems;
using UnityEngine;
using Utilities;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Prevent Game From Starting When UserID Is Missing")]
        [SerializeField] private bool PreventGameStartWithoutUserID;
        
        [Header("States")]
            private GameState _gameState;
            internal readonly GamePreparingState PreparingState = new();
            internal readonly GamePlayingLevelState PlayingLevelState = new();
            internal readonly GameLevelFinishedState LevelFinishedState = new();
        
        [field: Header("Level Setup GameObjects")]
            [field: SerializeField] public Transform EmojiSpawnPosition { get; private set; }
            [field: SerializeField] public GameObject ActionArea { get; private set; }

            public ScriptableLevel Level { get; private set; }
        

        public int SpawnedEmotesCount { get; private set; }
        

        private void OnEnable()
        {
            if (EditorUI.EditorUI.Instance.UserID == "")
            {
                Debug.LogWarning("No UserID Set");
                if (PreventGameStartWithoutUserID)
                    UnityEditor.EditorApplication.isPlaying = false;
                else
                    EditorUI.EditorUI.Instance.UserID = LoggingSystem.GetUnixTimestamp();
            }

            Level = EditorUI.EditorUI.Instance.GetSelectedLevel();
            
            EventManager.OnLevelStopped += OnLevelStoppedCallback;

            _gameState = PreparingState;
            SwitchState(PreparingState);
        }

        private void OnDestroy()
        {
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
            
            // Delete UserID after Game Ended
            EditorUI.EditorUI.Instance.ResetUserID();
        }

        private void OnLevelStoppedCallback()
        {
            SpawnedEmotesCount = 0;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
                OnButtonPressed(UIType.StartStopLevel);
        }

        public void SwitchState(GameState state)
        {
            _gameState.LeaveState();
            _gameState = state;
            _gameState.EnterState();
        }

        public bool CheckLevelEndConditions(int count)
        {
            switch (Level.LevelStruct.LevelMode)
            {
                case ELevelMode.Count:
                    if (count >= Level.LevelStruct.Count)
                        return true;
                    break;
                case ELevelMode.Predefined:
                    if (count >= Level.LevelStruct.EmoteArray.Length)
                        return true;
                    break;
                case ELevelMode.Training: // TODO: implement training end conditions
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public void OnButtonPressed(UIType uiType) => _gameState.HandleUIInput(uiType);
        
        public void OnButtonPressed(ScriptableLevel level) => _gameState.HandleUIInput(level);
        
        public void StopTimeScale() => StartCoroutine(MathHelper.SLerpTimeScale(1,0,1f));


        public void SetNewLevel(ScriptableLevel level)
        {
            Level = level;
            EditorUI.EditorUI.Instance.SetNewLevel(level);
        }

        public int GetLevelEmojiProgress() => PlayingLevelState.FinishedEmoteCount;

        public IEnumerable<EEmote> GetEmojiInActionArea() => PlayingLevelState.EmojiInActionArea;

        public int GetLevelScore() => PlayingLevelState.LevelScore;

        public void SetSpawnedEmotesCount(int spawnedEmotesCount) => SpawnedEmotesCount = spawnedEmotesCount;

        public void IncreaseSpawnedEmotesCount() => SpawnedEmotesCount++;

        public bool EmojisAreInActionArea() => PlayingLevelState.EmojiInActionArea.Any();
    }
}