namespace Scripts.Core
{
    public interface IStateMachine
    {
        void Enter<T>() where T : IState;
    }
}