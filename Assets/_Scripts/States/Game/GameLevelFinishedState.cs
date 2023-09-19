using System;
using Manager;
using Scriptables;

namespace States.Game
{
    public class GameLevelFinishedState : GameState
    {
        public override void EnterState()
        {
            EventManager.InvokeLevelFinished();
            GameManager.Instance.StopTimeScale();
            GameManager.Instance.ResetLevelState();
        }

        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.StartLevel:
                    break;
                case UIType.StopLevel:
                    break;
                case UIType.PauseLevel:
                    break;
                case UIType.ContinueEndScreen:
                    GameManager.Instance.SwitchState(GameManager.Instance.PreparingState);
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