using System.Collections.Generic;
using System;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.MenuFactory;
using BlindCrocodile.Services.Network;
using BlindCrocodile.Core.Services;
using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.Lobbies;

namespace BlindCrocodile.GameStates
{
    public class GameStateMachine : IStateMachine<IGameState>
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        // BootstrapState - initialize game
        // LoadMenuState
        // MenuState - join or host the game
        // LoadLobbyState - separete for host and clients (HostGameState/JoinGameState)
        // WaitingLobbyState - wait all the players to be ready. Start countdown and begin the game 
        // GameLoopState - handle game logic
        // RoundEndState - showroom for winners

        public GameStateMachine(SceneLoader sceneLoader, LoaderWidget loaderWidget, ServiceLocator services, ICoroutineRunner coroutineRunner)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, services, coroutineRunner),
                [typeof(LoadMenuState)] = new LoadMenuState(this, services.Single<IMenuFactory>(), sceneLoader, loaderWidget),
                [typeof(MenuState)] = new MenuState(this),
                [typeof(HostGameState)] = new HostGameState(this, services.Single<ILobbyFactory>(), services.Single<ILobbyService>(), services.Single<INetworkService>(), sceneLoader, loaderWidget),
                [typeof(JoinGameState)] = new JoinGameState(this, services.Single<ILobbyFactory>(), services.Single<ILobbyService>(), services.Single<INetworkService>(), sceneLoader, loaderWidget),
                [typeof(WaitingLobbyState)] = new WaitingLobbyState(this, sceneLoader),
                [typeof(GameLoopState)] = new GameLoopState(this),
            };
        }

        public void Enter<TState>() where TState : class, IGameState, IState
        {
            TState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IGameState, IPayloadedState<TPayload>
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
