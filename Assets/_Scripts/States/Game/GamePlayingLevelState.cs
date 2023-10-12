using System;
using Manager;
using Scriptables;

namespace States.Game
{
    public class GamePlayingLevelState : GameState
    {
        public override void EnterState()
        {
            EventManager.InvokeLevelStarted();
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