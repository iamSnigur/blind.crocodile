using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Multiplayer;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.Services.Relay;
using BlindCrocodile.Services;

namespace BlindCrocodile.Core
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly ServicesContainer _services;

        public BootstrapState(IStateMachine stateMachine, ServicesContainer services)
        {
            _stateMachine = stateMachine;
            _services = services;
            BindServices();
        }

        public void Enter() =>
            _stateMachine.Enter<LoadMenuState>();

        public void Exit() { }

        private void BindServices()
        {
            BindRelayService();
            BindMultiplayerService();
            BindStaticDataService();
            BindLobbyFactoryService();
        }

        private void BindLobbyFactoryService() =>
            _services.BindSingle<ILobbyFactory>(new LobbyFactory(_services.Single<IMultiplayerService>(), _services.Single<IStaticDataService>()));

        private void BindStaticDataService()
        {
            var staticDataService = new StaticDataService();
            staticDataService.LoadUI();
            _services.BindSingle<IStaticDataService>(staticDataService);
        }

        private void BindRelayService() =>
            _services.BindSingle<IRelayService>(new RelayService(Unity.Services.Relay.RelayService.Instance));

        private void BindMultiplayerService()
        {
            UnityTransport unityTransport = UnityEngine.Object.FindObjectOfType<UnityTransport>();
            _services.BindSingle<IMultiplayerService>(new MultiplayerService(unityTransport, NetworkManager.Singleton, _services.Single<IRelayService>()));
        }
    }
}