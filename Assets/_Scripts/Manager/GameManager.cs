using System.Collections.Generic;
using System.Linq;
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
        

        [Header("Level Progress")]
            private int _emojiCount;
            public int LevelEmojiProgress { get; private set; }
            public int LevelScore { get; private set; }
            public EEmote EmojiInActionArea { get; private set; }
        
        [Header("Webcams")]
            [SerializeField] private Webcam Webcam;
            [field: SerializeField] public bool ActivateWebcams { get; private set; }

            public ScriptableLevel Level { get; private set; }
        
        private void Start()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
            EventManager.OnEmojiFulfilled += OnEmojiFulfilledCallback;
            
            if (EditorUI.EditorUI.Instance.UserID == "")
                Debug.LogWarning("No UserID Set");

            Level = EditorUI.EditorUI.Instance.GetSelectedLevel();

            SwitchState(PreparingState);
            
        }

        private void OnDestroy()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
            EventManager.OnEmojiFulfilled -= OnEmojiFulfilledCallback;

            // Delete UserID after Game Ended
            EditorUI.EditorUI.Instance.ResetUserID();
        }

        private void OnEmoteEnteredAreaCallback(EEmote emote)
        {
            EmojiInActionArea = emote;
            SendRestImage();
        }

        private void SendRestImage()
        {
            string image = "";
            if (ActivateWebcams)
                image = Webcam.GetWebcamImage();
            REST.PostBase64(image);
        }
        
        private void OnEmojiFulfilledCallback(EEmote emote, float score)
        {
            LevelEmojiProgress++;
            LevelScore += 50 + (int)(score * 100);
        }

        private void OnEmoteExitedAreaCallback()
        {
            EmojiInActionArea = EEmote.None;
            
            _emojiCount++;
            if (_emojiCount >= Level.Count && Level.LevelMode == ELevelMode.Count)
            {        
                SwitchState(LevelFinishedState);
            }
        }

        public void ProcessRestResponse(Dictionary<EEmote, float>  response)
        {
            //LoggingSystem.Instance.WriteLog(response);
            EEmote maxEmote = response.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

#if UNITY_EDITOR
            EditorUI.EditorUI.SetRestResponseData(response);
#endif
          
            EventManager.InvokeEmotionDetected(maxEmote);
            
            if (EmojiInActionArea != EEmote.None)
            {
                SendRestImage();
            }
        }

        public void SwitchState(GameState state)
        {
            _gameState = state;
            _gameState.EnterState();
        }

        public void OnButtonPressed(UIType uiType) => _gameState.HandleUIInput(uiType);
        
        public void OnButtonPressed(ScriptableLevel level) => _gameState.HandleUIInput(level);
        
        public void StopTimeScale() => StartCoroutine(MathHelper.SLerpTimeScale(1,0,1f));

        public void ResetLevelState()
        {
            _emojiCount = 0;
            LevelEmojiProgress = 0;
            LevelScore = 0;
        }

        public void SetNewLevel(ScriptableLevel level)
        {
            Level = level;
            EditorUI.EditorUI.Instance.SetNewLevel(level);
        }
    }
}