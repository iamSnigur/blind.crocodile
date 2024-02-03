using BlindCrocodile.Core.StateMachine;

namespace BlindCrocodile.GameStates
{
    public class MenuState : IGameState, IState
    {
        private readonly AbstractStateMachine<IGameState> _stateMachine;

        public MenuState(AbstractStateMachine<IGameState> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter() { }

        public void Exit() { }
    }
}