using BlindCrocodile.Core.StateMachine;
using Scripts.GameplayStateMachine;
using System;
using System.Collections.Generic;

namespace BlindCrocodile.GameStates
{
    public class GameLoopState : IState
    {
        public GameLoopState(IStateMachine<IGameState> stateMachine)
        {

        }

        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }
    }

    // add network class to and state machine for players active state
    // can be

    // InLobbyState
    // ReadyState
    // PreRoundState
    // DrawingState
    // ComparisionState
    // RoundEndState
    public class GameplayStateMachine : IStateMachine<IGameplayState>
    {
        private readonly Dictionary<Type, IExitableState> _states;

        public GameplayStateMachine()
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(InLobbyState)] = new InLobbyState(),
            };
        }

        void IStateMachine<IGameplayState>.Enter<TState>()
        {
            throw new NotImplementedException();
        }

        void IStateMachine<IGameplayState>.Enter<TState, TPayload>(TPayload payload)
        {
            throw new NotImplementedException();
        }

        public class InLobbyState : IState
        {
            public void Enter()
            {

            }

            public void Exit()
            {
            }
        }
    }
}
