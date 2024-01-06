using BlindCrocodile.Core.StateMachine;

namespace BlindCrocodile.GameStates
{
    public class MenuState : IGameState, IState
    {
        private readonly IStateMachine<IGameState> _stateMachine;

        public MenuState(IStateMachine<IGameState> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter() { }

        public void Exit() { }
    }
}