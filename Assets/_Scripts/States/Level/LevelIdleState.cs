using Manager;
using UnityEngine;

namespace States.Level
{
    public class LevelIdleState : LevelState
    {
        public override void EnterState()
        {
            EventManager.InvokeGameStopped();
        }

        public override void LeaveState()
        {
            
        }
    }
}