using System;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using IUnityRelayService = Unity.Services.Relay.IRelayService;

namespace BlindCrocodile.Services.Relay
{
    public class RelayService : IRelayService
    {
        private readonly IUnityRelayService _unityRelayService;

        public RelayService(IUnityRelayService unityRelayService)
        {
            _unityRelayService = unityRelayService;
        }

        public async Task<Allocation> CreateAllocationAsync(int maxConnections, string region = null) => 
            await _unityRelayService.CreateAllocationAsync(maxConnections, region);

        public async Task<JoinAllocation> JoinAllocationAsync(string joinCode) =>
            await _unityRelayService.JoinAllocationAsync(joinCode);

        public async Task<string> GetJoinCodeAsync(Guid allocationId) => 
            await _unityRelayService.GetJoinCodeAsync(allocationId);
    }
}
