namespace BlindCrocodile.Core
{
    public class WaitingLobbyState : IState
    {
        private const string GAME_SCENE = "GameScene";

        private readonly IStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public WaitingLobbyState(IStateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.Load(GAME_SCENE);
        }

        public void Exit()
        {

        }
    }
}
