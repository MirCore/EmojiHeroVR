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
                case UIType.Start:
                    break;
                case UIType.Stop:
                    GameManager.Instance.SwitchState(GameManager.Instance.PreparingState);
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
            
        }
    }
}