using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Network;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.Services.Relay;
using BlindCrocodile.Services.MenuFactory;
using BlindCrocodile.Core.Services;
using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.Lobbies;
using UnityEngine;
using BlindCrocodile.Services.Factories;
using BlindCrocodile.NetworkStates;

namespace BlindCrocodile.GameStates
{
    public class BootstrapState : IGameState, IState
    {
        private readonly ServiceLocator _services;
        private readonly AbstractStateMachine<IGameState> _gameStateMachine;
        private readonly NetworkStateMachine _networkStateMachine;
        private readonly ICoroutineRunner _coroutineRunner;

        public BootstrapState(AbstractStateMachine<IGameState> gameStateMachine, ServiceLocator services, ICoroutineRunner coroutineRunner, NetworkStateMachine networkStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _networkStateMachine = networkStateMachine;
            _services = services;
            _coroutineRunner = coroutineRunner;
            BindServices();
        }

        public void Enter()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 143;
            _gameStateMachine.Enter<LoadMenuState>();
        }

        public void Exit() { }

        private void BindServices()
        {
            BindRelayService();
            BindLobbyService();
            BindNetworkService();
            BindStaticDataService();
            BindNetworkFactory();
            BindLobbyFactory(); // make one ui factory
            BindMenuFactory();
        }

        private void BindNetworkFactory() =>
            _services.BindSingle<INetworkFactory>(new NetworkFactory(_services.Single<IStaticDataService>(), _services.Single<INetworkService>()));

        private void BindMenuFactory() =>
            _services.BindSingle<IMenuFactory>(new MenuFactory(_services.Single<IStaticDataService>(), _gameStateMachine, _services.Single<ILobbyService>(), _networkStateMachine));

        private void BindLobbyFactory() =>
            _services.BindSingle<ILobbyFactory>(new LobbyFactory(_services.Single<INetworkService>(), _services.Single<IStaticDataService>(), _gameStateMachine, _services.Single<ILobbyService>(), _services.Single<INetworkFactory>()));

        private void BindStaticDataService()
        {
            var staticDataService = new StaticDataService();
            staticDataService.Load();
            _services.BindSingle<IStaticDataService>(staticDataService);
        }

        private void BindRelayService() =>
            _services.BindSingle<IRelayService>(new RelayService(Unity.Services.Relay.RelayService.Instance));

        private void BindLobbyService() =>
            _services.BindSingle<ILobbyService>(new LobbyService(Unity.Services.Lobbies.Lobbies.Instance, _coroutineRunner, _gameStateMachine));

        private void BindNetworkService()
        {
            UnityTransport unityTransport = Object.FindObjectOfType<UnityTransport>();
            NetworkService networkService = new(unityTransport, NetworkManager.Singleton, _services.Single<IRelayService>(), _services.Single<ILobbyService>()); // network manager
            _services.BindSingle<INetworkService>(networkService);
        }
    }
}