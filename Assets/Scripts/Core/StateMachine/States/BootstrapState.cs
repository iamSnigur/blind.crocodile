using BlindCrocodile.Services;
using BlindCrocodile.Services.Relay;

namespace BlindCrocodile.Core
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;

        public BootstrapState(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            InitializeServices();
        }

        public void Exit()
        {

        }

        private void InitializeServices()
        {
            ServicesContainer.SingleAs<IRelayService>(new RelayService(Unity.Services.Relay.RelayService.Instance));
        }
    }
}