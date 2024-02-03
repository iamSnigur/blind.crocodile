using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Factories;
using BlindCrocodile.NetworkStates;

namespace BlindCrocodile.GameStates
{
    public class JoinGameState : IGameState, IPayloadedState<string>
    {
        private const string GAME_SCENE = "GameScene";

        private readonly AbstractStateMachine<IGameState> _gameStateMachine;
        private readonly NetworkStateMachine _networkStateMachine;
        private readonly INetworkFactory _networkFactory;
        private readonly ILobbyFactory _lobbyFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public JoinGameState(AbstractStateMachine<IGameState> stateMachine, ILobbyFactory lobbyFactory, INetworkFactory networkFactory, SceneLoader sceneLoader, LoaderWidget loaderWidget, NetworkStateMachine networkStateMachine)
        {
            _gameStateMachine = stateMachine;
            _lobbyFactory = lobbyFactory;
            _sceneLoader = sceneLoader;
            _loaderWidget = loaderWidget;
            _networkFactory = networkFactory;
            _networkStateMachine = networkStateMachine;
        }

        public void Enter(string lobbyCode)
        {
            _loaderWidget.Show();
            _sceneLoader.Load(GAME_SCENE, delegate { OnLoaded(lobbyCode); });
        }

        public void Exit() =>
            _loaderWidget.Hide();

        private void OnLoaded(string lobbyCode)
        {
            _networkFactory.CreateNetworkPlayer();
            InitLobbyHudAsync(lobbyCode);
            _gameStateMachine.Enter<GameLoopState>();
            _networkStateMachine.Enter<JoinState, string>(lobbyCode);
        }

        private void InitLobbyHudAsync(string lobbyCode)
        {
            _lobbyFactory.CreateHud();
        }
    }
}
