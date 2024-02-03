using System.Collections.Generic;
using System;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.MenuFactory;
using BlindCrocodile.Core.Services;
using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.Services.Factories;
using BlindCrocodile.NetworkStates;

namespace BlindCrocodile.GameStates
{
    public class GameStateMachine : AbstractStateMachine<IGameState>
    {
        // BootstrapState - initialize game
        // LoadMenuState
        // MenuState - join or host the game
        // LoadLobbyState - separete for host and clients (HostGameState/JoinGameState)
        // WaitingLobbyState - wait all the players to be ready. Start countdown and begin the game 
        // GameLoopState - handle game logic
        // RoundEndState - showroom for winners

        public GameStateMachine(SceneLoader sceneLoader, LoaderWidget loaderWidget, ServiceLocator services, NetworkStateMachine networkStateMachine, ICoroutineRunner coroutineRunner)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, services, coroutineRunner, networkStateMachine),
                [typeof(LoadMenuState)] = new LoadMenuState(this, services.Single<IMenuFactory>(), sceneLoader, loaderWidget),
                [typeof(MenuState)] = new MenuState(this),
                [typeof(HostGameState)] = new HostGameState(this, services.Single<ILobbyFactory>(), services.Single<INetworkFactory>(), sceneLoader, loaderWidget, networkStateMachine),
                [typeof(JoinGameState)] = new JoinGameState(this, services.Single<ILobbyFactory>(), services.Single<INetworkFactory>(), sceneLoader, loaderWidget, networkStateMachine),
                [typeof(WaitingLobbyState)] = new WaitingLobbyState(this, sceneLoader),
                [typeof(GameLoopState)] = new GameLoopState(this),
            };
        }
    }
}
