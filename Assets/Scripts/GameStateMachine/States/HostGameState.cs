using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Factories;
using BlindCrocodile.NetworkStates;
using BlindCrocodile.Services.Network;

namespace BlindCrocodile.GameStates
{
    public class HostGameState : IGameState, IState
    {
        private const string GAME_SCENE = "GameScene";
        private const string LOBBY_NAME = "GameLobby";
        private const int MAX_CONNECTIONS = 5;

        private readonly AbstractStateMachine<IGameState> _gameStateMachine;
        private readonly NetworkStateMachine _networkStateMachine;
        private readonly ILobbyFactory _lobbyFactory;
        private readonly INetworkFactory _networkFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public HostGameState(AbstractStateMachine<IGameState> stateMachine, ILobbyFactory lobbyFactory, INetworkFactory networkFactory, SceneLoader sceneLoader, LoaderWidget loaderWidget, NetworkStateMachine networkStateMachine)
        {
            _gameStateMachine = stateMachine;
            _lobbyFactory = lobbyFactory;
            _sceneLoader = sceneLoader;
            _loaderWidget = loaderWidget;
            _networkFactory = networkFactory;
            _networkStateMachine = networkStateMachine;
        }

        public void Enter()
        {
            _loaderWidget.Show();
            _sceneLoader.Load(GAME_SCENE, OnLoaded);
        }

        public void Exit() =>
            _loaderWidget.Hide();

        private void OnLoaded()
        {
            NetworkPlayersLobby networkList = _networkFactory.CreateNetworkPlayer();
            _lobbyFactory.CreateHud();
            _gameStateMachine.Enter<GameLoopState>();
            _networkStateMachine.Enter<HostState, NetworkPlayersLobby>(networkList);
        }
    }
}