using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.Services.MenuFactory;

namespace BlindCrocodile.GameStates
{
    public class LoadMenuState : IGameState, IState
    {
        private const string MENU_SCENE = "LobbyScene";

        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly IMenuFactory _menuFactory;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public LoadMenuState(IStateMachine<IGameState> stateMachine, IMenuFactory lobbyFactory, SceneLoader sceneLoader, LoaderWidget loaderWidget)
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