using System;
using Enums;
using Manager;

namespace States.Game
{
    /// <summary>
    /// Represents the state of the game when a level is actively being played.
    /// </summary>
    public class GamePlayingLevelState : GameState
    {
        public override void EnterState()
        {
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }


        public override void LeaveState()
        {
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }

        private void OnLevelFinishedCallback()
        {
            GameManager.Instance.SwitchState(GameManager.Instance.LevelFinishedState);
        }

        /// <summary>
        /// Handle user interface input specific to the playing state.
        /// </summary>
        /// <param name="uiType">The type of UI interaction.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unexpected UIType is received.</exception>
        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.StopLevel:
                case UIType.StartStopLevel:
                    // Transition to the preparing state when the level is stopped.
                    GameManager.Instance.SwitchState(GameManager.Instance.LevelFinishedState);
                    break;
                case UIType.PauseLevel:
                case UIType.StartLevel:
                case UIType.Default:
                case UIType.ContinueEndScreen:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }
    }
}