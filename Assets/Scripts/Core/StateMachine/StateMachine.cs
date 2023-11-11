using Scipts.Core;
using System;
using System.Collections.Generic;

namespace Scripts.Core
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IState> _states;
        private IState _currentState;

        public StateMachine()
        {
            _states = new Dictionary<Type, IState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this),
                [typeof(LoadLevelState)] = new LoadLevelState(this),
            };
        }

        public void Enter<TState>() where TState : IState
        {
            _currentState?.Exit();
            _currentState = _states[typeof(TState)];
            _currentState?.Enter();
        }
    }
}
