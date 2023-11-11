namespace Scripts.Core
{
    public class Game
    {
        private readonly IStateMachine _stateMachine;

        public Game(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _stateMachine.Enter<BootstrapState>();
        }
    }
}