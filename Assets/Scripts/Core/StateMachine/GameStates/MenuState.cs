namespace BlindCrocodile.Core
{
    public class MenuState : IState
    {
        private readonly IStateMachine _stateMachine;

        // next state => LoadLobbyState
        // trigger when host or join game

        public MenuState(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {

        }

        public void Exit() { }
    }
}