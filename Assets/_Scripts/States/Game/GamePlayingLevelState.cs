using System;
using System.Collections.Generic;
using Enums;
using Manager;
using Scriptables;
using UnityEngine;

namespace States.Game
{
    public class GamePlayingLevelState : GameState
    {
        [Header("Level Progress")]
        private int _emojiCount;
        public int LevelEmojiProgress { get; private set; }
        public int LevelScore { get; private set; }
        public readonly List<EEmote> EmojiInActionArea = new();
        
        public override void EnterState()
        {
            EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
            EventManager.OnEmoteFailed += OnEmoteFailedCallback;
            EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;
            
            ResetLevelState();
            EventManager.InvokeLevelStarted();
        }

        public override void LeaveState()
        {
            EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
            EventManager.OnEmoteFailed -= OnEmoteFailedCallback;
            EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
            
            EmojiInActionArea.Clear();
        }

        private void ResetLevelState()
        {
            _emojiCount = 0;
            LevelEmojiProgress = 0;
            LevelScore = 0;
        }
        
        private void OnEmoteEnteredAreaCallback(EEmote emote)
        {
            EmojiInActionArea.Add(emote);
        }
        
        private void OnEmoteExitedAreaCallback(EEmote emote)
        {
            _emojiCount++;
            if (_emojiCount >= GameManager.Instance.Level.Count && GameManager.Instance.Level.LevelMode == ELevelMode.Count)
            {        
                GameManager.Instance.SwitchState(GameManager.Instance.LevelFinishedState);
            }
        }
        
        private void OnEmoteFailedCallback(EEmote emote)
        {
            EmojiInActionArea.Remove(emote);
        }
        
        private void OnEmoteFulfilledCallback(EEmote emote, float score)
        {
            EmojiInActionArea.Remove(emote);
            LevelEmojiProgress++;
            LevelScore += 50 + (int)(score * 10) * 10;
        }

        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.StartLevel:
                    break;
                case UIType.StopLevel:
                case UIType.StartStopLevel:
                    GameManager.Instance.SwitchState(GameManager.Instance.PreparingState);
                    break;
                case UIType.PauseLevel:
                    break;
                case UIType.Default:
                case UIType.ContinueEndScreen:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }

        public override void HandleUIInput(ScriptableLevel level)
        {
            
        }
    }
}