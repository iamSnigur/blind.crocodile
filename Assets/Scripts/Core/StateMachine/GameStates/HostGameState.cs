using BlindCrocodile.Core;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Network;
using System.Threading.Tasks;

namespace BlindCrocodile.StateMachine
{
    public class HostGameState : IState
    {
        private const string GAME_SCENE = "GameScene";
        private const string LOBBY_NAME = "GameLobby";
        private const int MAX_CONNECTIONS = 5;

        private readonly IStateMachine _gameStateMachine;
        private readonly ILobbyFactory _lobbyFactory;
        private readonly ILobbyService _lobbyService;
        private readonly INetworkService _networkService;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public HostGameState(IStateMachine stateMachine, ILobbyFactory lobbyFactory, ILobbyService lobbyService, INetworkService networkService, SceneLoader sceneLoader, LoaderWidget loaderWidget)
        {
            _gameStateMachine = stateMachine;
            _lobbyFactory = lobbyFactory;
            _sceneLoader = sceneLoader;
            _loaderWidget = loaderWidget;
            _lobbyService = lobbyService;
            _networkService = networkService;
        }

        public void Enter()
        {
            _loaderWidget.Show();
            _sceneLoader.Load(GAME_SCENE, OnLoaded);
        }

        public void Exit() =>
            _loaderWidget.Hide();

        private async void OnLoaded()
        {
            await InitLobbyHudAsync();
            _gameStateMachine.Enter<WaitingLobbyState>();
        }

        private async Task InitLobbyHudAsync()
        {
            _lobbyFactory.CreateHud();

            await _lobbyService.CreateLobbyAsync(LOBBY_NAME, MAX_CONNECTIONS);
            await _networkService.StartHostAsync(MAX_CONNECTIONS);
        }
    }
}
