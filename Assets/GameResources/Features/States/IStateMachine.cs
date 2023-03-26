namespace Game.Features.States
{
   public interface IStateMachine
   {
      public State CurrentState();

      public void Enter(State state);
   }
}
