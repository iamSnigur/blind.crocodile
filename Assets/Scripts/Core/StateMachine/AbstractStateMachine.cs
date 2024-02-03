using System;
using System.Collections.Generic;

namespace BlindCrocodile.Core.StateMachine
{
    public abstract class AbstractStateMachine<TBaseState>
    {
        protected Dictionary<Type, IExitableState> _states;
        protected IExitableState _activeState;

        public void Enter<TState>() where TState : class, TBaseState, IState
        {
            TState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, TBaseState, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
            TState state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;
    }
}