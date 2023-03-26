namespace Game.Features.States
{
    public class SimpleStateMachine : IStateMachine
    {
        private State _currentState { get; set; }

        State IStateMachine.CurrentState()
            => _currentState;

        public void Enter(State state)
            => _currentState = state;
    }

    public enum State
    {
        Start = 0,
        Game = 1,
        Death = 2,
        Pause = 3
    }
}
