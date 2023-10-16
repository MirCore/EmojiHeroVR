using System.Collections.Generic;
using Enums;
using Scriptables;
using States.Game;
using UnityEngine;
using Utilities;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("States")]
            private GameState _gameState;
            internal readonly GamePreparingState PreparingState = new();
            internal readonly GamePlayingLevelState PlayingLevelState = new();
            internal readonly GameLevelFinishedState LevelFinishedState = new();
        
        [field: Header("Level Setup GameObjects")]
            [field: SerializeField] public Transform EmojiSpawnPosition { get; private set; }
            [field: SerializeField] public GameObject ActionArea { get; private set; }
        
        
        [field: Header("Webcams")]
            [field: SerializeField] public Webcam Webcam { get; private set; }

            public ScriptableLevel Level { get; private set; }


        private void Start()
        {
            if (EditorUI.EditorUI.Instance.UserID == "")
                Debug.LogWarning("No UserID Set");

            Level = EditorUI.EditorUI.Instance.GetSelectedLevel();

            _gameState = PreparingState;
            SwitchState(PreparingState);
        }

        private void OnDestroy()
        {
            // Delete UserID after Game Ended
            EditorUI.EditorUI.Instance.ResetUserID();
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

        public void OnButtonPressed(UIType uiType) => _gameState.HandleUIInput(uiType);
        
        public void OnButtonPressed(ScriptableLevel level) => _gameState.HandleUIInput(level);
        
        public void StopTimeScale() => StartCoroutine(MathHelper.SLerpTimeScale(1,0,1f));


        public void SetNewLevel(ScriptableLevel level)
        {
            Level = level;
            EditorUI.EditorUI.Instance.SetNewLevel(level);
        }

        public int GetLevelEmojiProgress() => PlayingLevelState.LevelEmojiProgress;

        public IEnumerable<EEmote> GetEmojiInActionArea() => PlayingLevelState.EmojiInActionArea;

        public int GetLevelScore() => PlayingLevelState.LevelScore;
    }
}