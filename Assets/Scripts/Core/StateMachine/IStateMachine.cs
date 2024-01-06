namespace BlindCrocodile.Core.StateMachine
{
    public interface IStateMachine<TBaseState>
    {
        void Enter<TState>() where TState : class, TBaseState, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, TBaseState, IPayloadedState<TPayload>;
    }
}