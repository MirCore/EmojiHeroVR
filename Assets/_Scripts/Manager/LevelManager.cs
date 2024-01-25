using System;
using Enums;
using States.Level;
using Utilities;

namespace Manager
{
    public class LevelManager : Singleton<LevelManager>
    {
        // Level States
        private LevelState _levelState;
        internal readonly LevelIdleState IdleState = new();
        internal readonly LevelPreparingState PreparingState = new();
        internal readonly LevelPlayingState PlayingState = new();
        internal readonly LevelFinishedState FinishedState = new();
        public bool LevelIsPlaying => _levelState == PlayingState;


        private void OnEnable()
        {
            SwitchState(_levelState = IdleState);
        }

        internal void SwitchState(LevelState state)
        {
            _levelState.LeaveState();
            _levelState = state;
            _levelState.EnterState();
        }

        /// <summary>
        /// Checks if the current level's end conditions have been met.
        /// </summary>
        /// <param name="count">The current count of spawned or processed emojis.</param>
        /// <returns>True if the end conditions are met, false otherwise.</returns>
        public static bool CheckLevelEndConditions(int count)
        {
            switch (GameManager.Instance.Level.LevelMode)
            {
                case ELevelMode.Count:
                    if (count >= GameManager.Instance.Level.Count) // Check if Emoji count is reached
                        return true;
                    break;
                case ELevelMode.Predefined:
                    if (count >= GameManager.Instance.Level.EmoteArray.Length) // Check if all predefined Emojis have been spawned
                        return true;
                    break;
                case ELevelMode.Training: // TODO: implement training end conditions
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public void StartLevel()
        {
            if(_levelState == PreparingState)
                SwitchState(PlayingState);
        }

        public void PrepareLevel()
        {
            if(_levelState == IdleState)
                SwitchState(PreparingState);
        }
    }
}