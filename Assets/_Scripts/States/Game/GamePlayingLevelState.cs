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
        [field: Header("Level Progress")]
        public int FinishedEmoteCount { get; private set; }

        private int FulfilledEmoteCount { get; set; }
        public int SpawnedEmotesCount { get; internal set; }
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

            ResetLevelState();
        }

        private void ResetLevelState()
        {
            FinishedEmoteCount = 0;
            FulfilledEmoteCount = 0;
            LevelScore = 0;
            SpawnedEmotesCount = 0;
            EmojiInActionArea.Clear();
        }
        
        private void OnEmoteEnteredAreaCallback(EEmote emote)
        {
            EmojiInActionArea.Add(emote);
        }
        
        private void OnEmoteExitedAreaCallback(EEmote emote)
        {
            EmojiInActionArea.Remove(emote);
            FinishedEmoteCount++;
            if (GameManager.Instance.CheckLevelEndConditions(FinishedEmoteCount))
                GameManager.Instance.SwitchState(GameManager.Instance.LevelFinishedState);
        }
        
        private void OnEmoteFailedCallback(EEmote emote)
        {
        }
        
        private void OnEmoteFulfilledCallback(EEmote emote, float score)
        {
            FulfilledEmoteCount++;
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