using BlindCrocodile.Services.LobbyFactory;

namespace BlindCrocodile.Core
{
    public class LoadMenuState : IState
    {
        private const string MENU_SCENE = "LobbyScene";

        private readonly IStateMachine _stateMachine;
        private readonly ILobbyFactory _lobbyFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public LoadMenuState(IStateMachine stateMachine, ILobbyFactory lobbyFactory, SceneLoader sceneLoader, LoaderWidget loaderWidget)
        {
            _stateMachine = stateMachine;
            _lobbyFactory = lobbyFactory;
            _sceneLoader = sceneLoader;
            _loaderWidget = loaderWidget;
        }

        public void Enter()
        {
            _loaderWidget.Show();
            _sceneLoader.Load(MENU_SCENE, OnSceneLoad);
        }

        public void Exit() =>
            _loaderWidget.Hide();

        private void OnSceneLoad()
        {
            InitLobbyHud();
            _stateMachine.Enter<MenuState>();
        }

        private void InitLobbyHud() =>
            _lobbyFactory.CreateHub();
    }
}