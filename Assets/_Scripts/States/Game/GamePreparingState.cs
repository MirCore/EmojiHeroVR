using System;
using Manager;
using Scriptables;
using UnityEngine;

namespace States.Game
{
    public class GamePreparingState : GameState
    {
        public override void EnterState()
        {
            Time.timeScale = 1;
            EventManager.InvokeLevelStopped();
        }

        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.StartLevel:
                case UIType.StartStopLevel:
                    GameManager.Instance.SwitchState(GameManager.Instance.PlayingLevelState);
                    break;
                case UIType.StopLevel:
                    break;
                case UIType.PauseLevel:
                    break;
                case UIType.Default:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }

        public override void HandleUIInput(ScriptableLevel level)
        {
            GameManager.Instance.SetNewLevel(level);
        }
    }
}