using System;
using Enums;
using Manager;
using UnityEngine;

namespace States.Game
{
    public class GamePreparingState : GameState
    {
        public override void EnterState()
        {
            
        }

        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.Start:
                    GameManager.Instance.SwitchState(GameManager.Instance.PlayingLevelState);
                    EventManager.InvokeLevelStarted();
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
    }
}