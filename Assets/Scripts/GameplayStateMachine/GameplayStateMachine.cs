using BlindCrocodile.Core.StateMachine;
using Scripts.GameplayStateMachine;

namespace BlindCrocodile.GameplayStateMachine
{
    public class GameplayStateMachine : IStateMachine<IGameplayState>
    {
        public void Enter<TState>() where TState : class, IGameplayState, IState
        {

        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IGameplayState, IPayloadedState<TPayload>
        {

        }
    }
}
