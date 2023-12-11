using System;
using Enums;
using Manager;
using UnityEngine.SceneManagement;

namespace States.Game
{
    /// <summary>
    /// Represents the state of the game when a level has finished, handling the transition and user interactions during this period.
    /// </summary>
    public class GameLevelFinishedState : GameState
    {
        /// <summary>
        /// Actions to perform when entering the level finished state.
        /// </summary>
        public override void EnterState()
        {
            EventManager.OnLevelStopped += OnLevelStoppedCallback;
        }

        public override void LeaveState()
        {
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
        }

        private void OnLevelStoppedCallback()
        {
            GameManager.Instance.SwitchState(GameManager.Instance.PreparingState);
        }

        /// <summary>
        /// Handle user interface input specific to the finished state.
        /// </summary>
        /// <param name="uiType">The type of UI interaction.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unexpected UIType is received.</exception>
        public override void HandleUIInput(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.ContinueEndScreen:
                case UIType.StartStopLevel:
                    GameManager.Instance.SwitchState(GameManager.Instance.MenuState);
                    break;
                case UIType.StartLevel:
                case UIType.StopLevel:
                case UIType.PauseLevel:
                case UIType.Default:
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiType), uiType, null);
            }
        }
    }
}