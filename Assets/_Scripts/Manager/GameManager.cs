using Enums;
using Scriptables;
using States.Game;
using Systems;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

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
            [field: SerializeField] public Transform EmojiEndPosition { get; private set; }
            [field: SerializeField] public GameObject ActionArea { get; private set; }
        
        [field: Header("Selected Level")]
            [field: SerializeField] public ScriptableLevel Level { get; internal set; }

        [Header("Level Progress")]
            private int _emojiCount;
            public int LevelEmojiProgress { get; private set; }
            public int LevelScore { get; private set; }
            public EEmote EmojiInActionArea { get; private set; }
        
        [Header("Webcams")]
            [SerializeField] private Webcam Webcam;
            [field: SerializeField] public bool ActivateWebcams { get; private set; }

        private void Start()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
            EventManager.OnEmojiFulfilled += OnEmojiFulfilledCallback;

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
            EmojiInActionArea = emote;
            SendRestImage();
        }

        private void SendRestImage()
        {
            string image = "";
            //Rest.Post();
            if (ActivateWebcams)
                image = Webcam.GetWebcamImage();
            REST.FakePost(image, Random.Range(0.1f,0.7f));
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

        public void ProcessRestResponse(RestPost response)
        {
            LoggingSystem.Instance.WriteLog(response);
            
            if (response.Result)
            {
                EventManager.InvokeEmotionDetected(EmojiInActionArea);
            }

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
    }
}