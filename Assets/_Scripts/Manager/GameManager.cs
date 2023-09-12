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
        
        public EGameState GameState { get; set; }
        [field: SerializeField] public Transform EmojiEndPosition { get; private set; }
        [field: SerializeField] public Transform ActiveAreaStartPosition { get; private set; }
        [field: SerializeField] public Transform ActiveAreaEndPosition { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float SpawnInterval { get; private set; }
        [field: SerializeField] public bool SpawnActive { get; private set; }
        
        [field: SerializeField] public ScriptableLevel Level { get; private set; }
        private int _emojiCount;

        private REST _rest;

        private EEmote _emojiInActionArea;

        private void Start()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;

            _rest = new REST();
            _gameState = PreparingState;
            _gameState.EnterState();
        }


        private void OnDestroy()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
        }

        private void OnEmoteEnteredAreaCallback(EEmote emote)
        {
            _emojiInActionArea = emote;
            SendRestImage();
        }

        private void SendRestImage()
        {
            //_rest.Post();
            _rest.FakePost(Random.Range(0.1f,0.7f));
        }

        private void OnEmoteExitedAreaCallback()
        {
            _emojiInActionArea = EEmote.None;
        }

        public void ProcessRestResponse(Post response)
        {
            if (response.result)
            {
                Debug.Log(response.result.ToString());
                EventManager.InvokeEmotionDetected(_emojiInActionArea);
            }

            if (_emojiInActionArea != EEmote.None)
            {
                SendRestImage();
            }
        }

        public void OnStartButtonPressed()
        {
            _gameState.HandleUIInput(UIType.Start);
        }

        public void SwitchState(GameState state)
        {
            _gameState = state;
            _gameState.EnterState();
        }
    }
}