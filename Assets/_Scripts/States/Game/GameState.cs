using Scriptables;

namespace States.Game
{
    public abstract class GameState
    {
        public abstract void EnterState();
        public abstract void LeaveState();
        public abstract void HandleUIInput(UIType uiType);
        public abstract void HandleUIInput(ScriptableLevel level);

    }
}