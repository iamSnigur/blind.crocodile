namespace BlindCrocodile.Core
{
    public interface IStateMachine
    {
        void Enter<TState>() where TState : IState;
    }
}