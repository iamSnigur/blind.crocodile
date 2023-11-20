namespace BlindCrocodile.Core
{
    public class MenuState : IState
    {
        private readonly IStateMachine _stateMachine;

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