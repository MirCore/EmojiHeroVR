﻿using System;
using Manager;
using Scriptables;

namespace States.Game
{
    public class GamePreparingState : GameState
    {
        public override void EnterState()
        {
            EventManager.InvokeLevelStopped();
        }

        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.Start:
                    GameManager.Instance.SwitchState(GameManager.Instance.PlayingLevelState);
                    break;
                case UIType.Stop:
                    break;
                case UIType.Pause:
                    break;
                case UIType.Default:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }

        public override void HandleUIInput(ScriptableLevel level)
        {
            GameManager.Instance.Level = level;
        }
    }
}