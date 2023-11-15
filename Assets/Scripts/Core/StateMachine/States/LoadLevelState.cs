namespace BlindCrocodile.Core
{
    public class LoadLevelState : IState
    {
        private readonly IStateMachine _stateMachine;

        public LoadLevelState(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}