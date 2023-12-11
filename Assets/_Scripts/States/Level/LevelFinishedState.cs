using Manager;

namespace States.Level
{
    public class LevelFinishedState : LevelState
    {
        public override void EnterState()
        {
            // Notify other parts of the game that the level has finished.
            EventManager.InvokeLevelFinished();
            
            // Pause the game's time scale, effectively pausing the game.
            GameManager.Instance.StopTimeScale();
        }

        public override void LeaveState()
        {
            
        }
    }
}