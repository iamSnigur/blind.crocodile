using BlindCrocodile.Core;

namespace BlindCrocodile.StateMachine
{
    public class LoadLobbyState : IPayloadedState<string>
    {
        private readonly IStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoaderWidget _loaderWidget;

        public LoadLobbyState(IStateMachine stateMachine, SceneLoader sceneLoader, LoaderWidget loaderWidget)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _loaderWidget = loaderWidget;
        }

        public void Enter(string payload)
        {
            _loaderWidget.Show();
            _sceneLoader.Load(payload);
        }

        public void Exit() =>
            _loaderWidget.Hide();
    }
}
