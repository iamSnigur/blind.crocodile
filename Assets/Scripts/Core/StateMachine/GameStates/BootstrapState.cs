using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Network;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.Services.Relay;
using BlindCrocodile.Core.Services;
using BlindCrocodile.Services.MenyFactory;
using BlindCrocodile.Lobbies;

namespace BlindCrocodile.Core
{
    public class BootstrapState : IState
    {
        private readonly ServicesContainer _services;
        private readonly IStateMachine _stateMachine;
        private readonly ICoroutineRunner _coroutineRunner;

        public BootstrapState(IStateMachine stateMachine, ServicesContainer services, ICoroutineRunner coroutineRunner)
        {
            _stateMachine = stateMachine;
            _services = services;
            _coroutineRunner = coroutineRunner;
            BindServices();
        }

        public void Enter() =>
            _stateMachine.Enter<LoadMenuState>();

        public void Exit() { }

        private void BindServices()
        {
            BindRelayService();
            BindLobbyService();
            BindNetworkService();
            BindStaticDataService();
            BindLobbyFactoryService();
            BindMenuFactoryService();
        }

        private void BindLobbyFactoryService() =>
            _services.BindSingle<ILobbyFactory>(new LobbyFactory(_services.Single<INetworkService>(), _services.Single<IStaticDataService>(), _stateMachine, _services.Single<ILobbyService>()));

        private void BindMenuFactoryService() =>
            _services.BindSingle<IMenuFactory>(new MenuFactory(_services.Single<IStaticDataService>(), _stateMachine, _services.Single<ILobbyService>()));

        private void BindStaticDataService()
        {
            var staticDataService = new StaticDataService();
            staticDataService.LoadUI();
            _services.BindSingle<IStaticDataService>(staticDataService);
        }

        private void BindRelayService() =>
            _services.BindSingle<IRelayService>(new RelayService(Unity.Services.Relay.RelayService.Instance));

        private void BindLobbyService() =>
            _services.BindSingle<ILobbyService>(new LobbyService(Unity.Services.Lobbies.Lobbies.Instance, _coroutineRunner));

        private void BindNetworkService()
        {
            UnityTransport unityTransport = UnityEngine.Object.FindObjectOfType<UnityTransport>();
            NetworkService networkService = new(unityTransport, NetworkManager.Singleton, _services.Single<IRelayService>(), _services.Single<ILobbyService>());
            _services.BindSingle<INetworkService>(networkService);
        }
    }
}