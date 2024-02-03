using BlindCrocodile.Core.StateMachine;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using BlindCrocodile.Core.Services;
using BlindCrocodile.Services.Network;
using System.Text;
using BlindCrocodile.Lobbies;
using System.Threading.Tasks;
using UnityEngine;
using BlindCrocodile.Core;
using NetworkPlayer = BlindCrocodile.Services.Network.NetworkPlayer;

namespace BlindCrocodile.NetworkStates
{
    public class NetworkStateMachine : AbstractStateMachine<INetworkState>
    {
        public void Construct(ServiceLocator services, NetworkManager networkManager, LoaderWidget loaderWidget)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(OfflineState)] = new OfflineState(),
                [typeof(HostState)] = new HostState(networkManager, this, services.Single<ILobbyService>(), services.Single<INetworkService>(), loaderWidget),
                [typeof(JoinState)] = new JoinState(networkManager, this, services.Single<ILobbyService>(), services.Single<INetworkService>(), loaderWidget),
            };
        }
    }

    public class HostState : INetworkState, IPayloadedState<NetworkPlayersLobby> // StartHostState | HostingState ?
    {
        private const string LOBBY_NAME = "GameLobby";
        private const int MAX_CONNECTIONS = 5;

        private readonly NetworkStateMachine _networkStateMachine;
        private readonly NetworkManager _networkManager;
        private readonly INetworkService _networkService;
        private readonly ILobbyService _lobbyService;
        private readonly LoaderWidget _loaderWidget;

        private NetworkPlayersLobby _networkPlayerView;

        public HostState(NetworkManager networkManager, NetworkStateMachine networkStateMachine, ILobbyService lobbyService, INetworkService networkService, LoaderWidget loaderWidget)
        {
            _networkManager = networkManager;
            _networkStateMachine = networkStateMachine;
            _lobbyService = lobbyService;
            _networkService = networkService;
            _loaderWidget = loaderWidget;
        }

        public async void Enter(NetworkPlayersLobby payload)
        {
            Debug.Log("HostState");
            _networkPlayerView = payload;
            // sub on server callbacks
            _networkManager.ConnectionApprovalCallback += OnApprovalCheck;
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnServerStopped += OnServerStoped;

            // do some connection stuf
            //_loaderWidget.Show();
            await StartHostAsync();
            //_loaderWidget.Hide();
        }

        public void Exit()
        {
            // unsub on server callbacks
            _networkManager.ConnectionApprovalCallback -= OnApprovalCheck;
            _networkManager.OnClientConnectedCallback -= OnClientConnected;
            _networkManager.OnServerStopped -= OnServerStoped;
        }

        private async Task StartHostAsync()
        {
            try
            {
                await _lobbyService.CreateLobbyAsync(LOBBY_NAME, MAX_CONNECTIONS);
                await _networkService.StartHostAsync(MAX_CONNECTIONS);
            }
            catch
            {
                SessionData.ClearPlayers();
                _networkStateMachine.Enter<OfflineState>();
            }
        }

        private void OnServerStoped(bool _)
        {
            SessionData.ClearPlayers();
            _networkStateMachine.Enter<OfflineState>();
        }

        private void OnApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            string lobbyId = Encoding.UTF8.GetString(request.Payload);
            SessionData.AddPlayer(request.ClientNetworkId, lobbyId);
            Debug.Log("connection approved");
            response.Approved = true;
            response.Pending = false;
        }

        private void OnClientConnected(ulong clientId)
        {
            // add player into network list
            Debug.Log("client connected " + clientId);
            string lobbyId = SessionData.ConnectedPlayerIds[clientId];
            Debug.Log("client lobby id: " + lobbyId);

            _networkPlayerView.Players.Add(
                new NetworkPlayer(
                    clientId,
                    _lobbyService.LocalLobby.Players[lobbyId].Name,
                    lobbyId,
                    _lobbyService.LocalLobby.Players[lobbyId].IsHost,
                    PlayerRole.Guesser));
        }
    }

    public class JoinState : INetworkState, IPayloadedState<string>
    {
        private readonly NetworkManager _networkManager;
        private readonly NetworkStateMachine _networkStateMachine;
        private readonly INetworkService _networkService;
        private readonly ILobbyService _lobbyService;
        private readonly LoaderWidget _loaderWidget;

        public JoinState(NetworkManager networkManager, NetworkStateMachine networkStateMachine, ILobbyService lobbyService, INetworkService networkService, LoaderWidget loaderWidget)
        {
            _networkManager = networkManager;
            _networkStateMachine = networkStateMachine;
            _lobbyService = lobbyService;
            _networkService = networkService;
            _loaderWidget = loaderWidget;
        }

        public async void Enter(string lobbyCode)
        {
            Debug.Log("JoinState");
            _networkManager.OnServerStopped += OnServerStoped;

            //_loaderWidget.Show();
            await JoinServerAsync(lobbyCode);
            //_loaderWidget.Hide();
        }

        private async Task JoinServerAsync(string lobbyCode)
        {
            await _lobbyService.JoinLobbyByCodeAsync(lobbyCode);
            await _networkService.JoinServerAsync();
        }

        public void Exit()
        {
            _networkManager.OnServerStopped -= OnServerStoped;
        }

        private void OnServerStoped(bool _)
        {
            _networkStateMachine.Enter<OfflineState>();
        }
    }

    public abstract class AbstractOnlineState<TPayload> : INetworkState, IPayloadedState<TPayload>
    {
        public abstract void Enter(TPayload payload);

        public abstract void Exit();
    }
}