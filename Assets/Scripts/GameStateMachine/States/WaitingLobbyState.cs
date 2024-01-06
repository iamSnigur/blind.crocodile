using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;

namespace BlindCrocodile.GameStates
{
    public class WaitingLobbyState : IGameState, IState
    {
        private const string GAME_SCENE = "GameScene";

        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public WaitingLobbyState(IStateMachine<IGameState> stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
        }

        public void Exit()
        {

        }
    }
}
