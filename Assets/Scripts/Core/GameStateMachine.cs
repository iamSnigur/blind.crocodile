using BlindCrocodile.Services;
using System.Collections.Generic;
using System;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.StateMachine;

namespace BlindCrocodile.Core
{
    public class GameStateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        // BootstrapState => LobbyMenuState => WaitingLobbyState => GameLoopState

        // BootstrapState - initialize game
        // LoadMenuState
        // MenuState - join or host the game
        // LoadLobbyState
        // WaitingLobbyState - wait all the players to be ready. Start countdown and begin the game 
        // GameLoopState - handle game logic
        // RoundEndState - showroom for winners

        public GameStateMachine(SceneLoader sceneLoader, LoaderWidget loaderWidget, ServicesContainer services)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, services),
                [typeof(LoadMenuState)] = new LoadMenuState(this, services.Single<ILobbyFactory>(), sceneLoader, loaderWidget),
                [typeof(MenuState)] = new MenuState(this),
                [typeof(LoadLobbyState)] = new LoadLobbyState(this, sceneLoader, loaderWidget),
                [typeof(WaitingLobbyState)] = new WaitingLobbyState(this, sceneLoader),
                [typeof(GameLoopState)] = new GameLoopState(this),
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            TState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
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
