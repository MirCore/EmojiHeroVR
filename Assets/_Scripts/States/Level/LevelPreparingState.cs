using Manager;

namespace States.Level
{
    public class LevelPreparingState : LevelState
    {
        public override void EnterState()
        {
            GameManager.Instance.RestartTimeScale();
            EventManager.InvokeLevelStopped();
        }

        public override void LeaveState()
        {
            
        }
    }
}