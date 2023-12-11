using System;
using Enums;
using Manager;

namespace States.Game
{
    public class GameMenuState : GameState
    {
        public override void EnterState()
        {
            EventManager.InvokeMenuOpened();
        }

        public override void LeaveState()
        {
            
        }

        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.StartLevel:
                    GameManager.Instance.SwitchState(GameManager.Instance.PreparingState);
                    break;
                case UIType.ContinueEndScreen:
                case UIType.StartStopLevel:
                case UIType.StopLevel:
                case UIType.PauseLevel:
                case UIType.Default:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }
    }
}