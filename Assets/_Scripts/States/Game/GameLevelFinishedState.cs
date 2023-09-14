using System;
using Manager;
using Scriptables;
using UnityEngine;

namespace States.Game
{
    public class GameLevelFinishedState : GameState
    {
        public override void EnterState()
        {
            EventManager.InvokeLevelFinished();
            Time.timeScale = 0; // TODO: Lerp
        }

        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.Start:
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
            
        }
    }
}