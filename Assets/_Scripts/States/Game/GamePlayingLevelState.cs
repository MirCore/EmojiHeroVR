using System;
using Enums;
using Manager;

namespace States.Game
{
    public class GamePlayingLevelState : GameState
    {
        public override void EnterState()
        {
            
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
    }
}