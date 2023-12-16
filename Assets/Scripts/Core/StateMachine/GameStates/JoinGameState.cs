using BlindCrocodile.Core;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Network;
using System.Threading.Tasks;
using UnityEngine;

namespace BlindCrocodile.StateMachine
{
    public class JoinGameState : IPayloadedState<string>
    {
        private const string GAME_SCENE = "GameScene";

        private readonly IStateMachine _gameStateMachine;
        private readonly ILobbyService _lobbyService;
        private readonly INetworkService _networkService;
        private readonly ILobbyFactory _lobbyFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public JoinGameState(IStateMachine stateMachine, ILobbyFactory lobbyFactory, ILobbyService lobbyService, INetworkService networkService, SceneLoader sceneLoader, LoaderWidget loaderWidget)
        {
            _gameStateMachine = stateMachine;
            _lobbyFactory = lobbyFactory;
            _sceneLoader = sceneLoader;
            _loaderWidget = loaderWidget;
            _lobbyService = lobbyService;
            _networkService = networkService;
        }

        public void Enter(string lobbyCode)
        {
            _loaderWidget.Show();
            _sceneLoader.Load(GAME_SCENE, delegate { OnLoaded(lobbyCode); });
        }

        public void Exit() =>
            _loaderWidget.Hide();

        private async void OnLoaded(string lobbyCode)
        {
            await InitLobbyHudAsync(lobbyCode);
            _gameStateMachine.Enter<WaitingLobbyState>();
        }

        private async Task InitLobbyHudAsync(string lobbyCode)
        {
            _lobbyFactory.CreateHud();

            await _lobbyService.JoinLobbyByCodeAsync(lobbyCode);
            await _networkService.JoinServerAsync();
        }
    }
}
