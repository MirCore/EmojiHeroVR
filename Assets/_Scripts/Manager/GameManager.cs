using System;
using Enums;
using Scriptables;
using States.Game;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        private GameState _gameState;
        internal readonly GamePreparingState PreparingState = new();
        internal readonly GamePlayingLevelState PlayingLevelState = new();
        internal readonly GameLevelFinishedState LevelFinishedState = new();
        
        [field: SerializeField] public Transform EmojiEndPosition { get; private set; }
        [field: SerializeField] public GameObject ActionArea { get; private set; }
        
        [field: SerializeField] public ScriptableLevel Level { get; internal set; }
        private int _emojiCount;
        public int LevelEmojiProgress { get; private set; }
        public int LevelScore { get; private set; }

        private REST _rest;

        private EEmote _emojiInActionArea;

        [SerializeField] private Webcam Webcam;

        private void Start()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
            EventManager.OnEmojiFulfilled += OnEmojiFulfilledCallback;

            _rest = new REST();
            SwitchState(PreparingState);
        }

        private void OnDestroy()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
            EventManager.OnEmojiFulfilled -= OnEmojiFulfilledCallback;
        }

        private void OnEmoteEnteredAreaCallback(EEmote emote)
        {
            _emojiInActionArea = emote;
            SendRestImage();
        }

        private void SendRestImage()
        {
            //_rest.Post();
            string image = Webcam.GetWebcamImage();
            REST.FakePost(image, Random.Range(0.1f,0.7f));
        }
        
        private void OnEmojiFulfilledCallback(EEmote emote, float score)
        {
            LevelEmojiProgress++;
            LevelScore += (int)(score * 100);
        }

        private void OnEmoteExitedAreaCallback()
        {
            _emojiInActionArea = EEmote.None;
            
            _emojiCount++;
            if (_emojiCount >= Level.Count && Level.LevelMode == ELevelMode.Count)
            {        
                SwitchState(LevelFinishedState);
            }
        }

        public void ProcessRestResponse(Post response)
        {
            if (response.Result)
            {
                EventManager.InvokeEmotionDetected(_emojiInActionArea);
            }

            if (_emojiInActionArea != EEmote.None)
            {
                SendRestImage();
            }
        }

        public void SwitchState(GameState state)
        {
            _gameState = state;
            _gameState.EnterState();
        }

        public void OnButtonPressed(UIType uiType)
        {
            _gameState.HandleUIInput(uiType);
        }
        
        public void OnButtonPressed(ScriptableLevel level)
        {
            _gameState.HandleUIInput(level);
        }
    }
}