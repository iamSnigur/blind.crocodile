using BlindCrocodile.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using System.Threading.Tasks;

namespace BlindCrocodile.Services.Multiplayer
{
    public class MultiplayerService : IMultiplayerService
    {
        private readonly UnityTransport _unityTransport;
        private readonly NetworkManager _networkManager;
        private readonly IRelayService _relayService;

        public MultiplayerService(UnityTransport unityTransport, NetworkManager networkManager, IRelayService relayService)
        {
            _unityTransport = unityTransport;
            _networkManager = networkManager;
            _relayService = relayService;
        }

        public async Task<string> StartHost(int maxConnections)
        {
            Allocation allocation = await _relayService.CreateAllocationAsync(maxConnections);
            string joinCode = await _relayService.GetJoinCodeAsync(allocation.AllocationId);

            _unityTransport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData);

            _networkManager.StartHost();

            return joinCode;
        }

        public async void JoinServer(string joinCode)
        {
            JoinAllocation joinAllocation = await _relayService.JoinAllocationAsync(joinCode);

            _unityTransport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);

            _networkManager.StartClient();
        }

        public void Disconnect() => 
            _networkManager.DisconnectClient(_networkManager.LocalClientId);
    }
}
