using BlindCrocodile.Services.Relay;
using BlindCrocodile.Lobbies;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;

namespace BlindCrocodile.Services.Network
{
    public class NetworkService : INetworkService
    {
        private const string DTLS_CONNECTION = "dtls";

        public bool IsServer => _networkManager.IsServer;
        public bool IsClient => _networkManager.IsClient;
        public ulong ClientId => _networkManager.LocalClientId;

        private readonly UnityTransport _unityTransport;
        private readonly NetworkManager _networkManager;
        private readonly IRelayService _relayService;
        private readonly ILobbyService _lobbyService;

        public NetworkService(UnityTransport unityTransport, NetworkManager networkManager, IRelayService relayService, ILobbyService lobbyService)
        {
            _unityTransport = unityTransport;
            _networkManager = networkManager;
            _relayService = relayService;
            _lobbyService = lobbyService;
            _lobbyService.LocalPlayer.NetworkId = ClientId;
            _networkManager.NetworkConfig.ConnectionApproval = true;            
        }

        public async Task StartHostAsync(int maxConnections)
        {
            Allocation allocation = await _relayService.CreateAllocationAsync(maxConnections);
            string relayCode = await _relayService.GetJoinCodeAsync(allocation.AllocationId);
            _lobbyService.LocalLobby.RelayCode = relayCode;

            await _lobbyService.UpdateRemoteLobbyDataAsync();
            await _lobbyService.UpdateRemotePlayerDataAsync(allocation.AllocationId.ToString(), relayCode);

            _unityTransport.SetRelayServerData(new RelayServerData(allocation, DTLS_CONNECTION));

            _networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(_lobbyService.LocalPlayer.Id);

            _networkManager.StartHost();
        }

        public async Task JoinServerAsync()
        {
            string relayCode = _lobbyService.LocalLobby.RelayCode;

            if (string.IsNullOrEmpty(relayCode))
                return;

            JoinAllocation joinAllocation = await _relayService.JoinAllocationAsync(relayCode);

            await _lobbyService.UpdateRemotePlayerDataAsync(joinAllocation.AllocationId.ToString(), relayCode);

            _unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, DTLS_CONNECTION));
            _networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(_lobbyService.LocalPlayer.Id);
            _networkManager.StartClient();
        }

        public void Disconnect()
        {
            _lobbyService.DisconnectFromLobby();
            _networkManager.Shutdown();
        }

        public void AddNetworkPrefab(GameObject gameObject)
        {
            _networkManager.AddNetworkPrefab(gameObject);
        }
    }

    public record PayloadData // data for connection to server
    {

    }
}
