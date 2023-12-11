namespace States.Level
{
    public abstract class LevelState
    {
        /// <summary>
        /// Called when the level enters this state.
        /// </summary>
        public abstract void EnterState();

        /// <summary>
        /// Called when the level leaves this state.
        /// </summary>
        /// 
        public abstract void LeaveState();
    }
}