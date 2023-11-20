namespace BlindCrocodile.Core
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}