using BlindCrocodile.Services.MenyFactory;

namespace BlindCrocodile.Core
{
    public class LoadMenuState : IState
    {
        private const string MENU_SCENE = "LobbyScene";

        private readonly IStateMachine _stateMachine;
        private readonly IMenuFactory _menuFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public LoadMenuState(IStateMachine stateMachine, IMenuFactory lobbyFactory, SceneLoader sceneLoader, LoaderWidget loaderWidget)
        {
            _stateMachine = stateMachine;
            _menuFactory = lobbyFactory;
            _sceneLoader = sceneLoader;
            _loaderWidget = loaderWidget;
        }

        public void Enter()
        {
            _loaderWidget.Show();
            _sceneLoader.Load(MENU_SCENE, OnLoaded);
        }

        public void Exit() =>
            _loaderWidget.Hide();

        private void OnLoaded()
        {
            InitMenuHud();
            _stateMachine.Enter<MenuState>();
        }

        private void InitMenuHud() =>
            _menuFactory.CreateHud();
    }
}