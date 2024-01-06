namespace BlindCrocodile.Core.StateMachine
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}