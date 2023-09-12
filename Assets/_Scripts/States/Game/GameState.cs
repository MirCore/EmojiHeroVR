namespace States.Game
{
    public abstract class GameState
    {
        public abstract void EnterState();
        public abstract void HandleUIInput(UIType uiType);
    }
}