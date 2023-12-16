using BlindCrocodile.Core.Services;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using System;

namespace BlindCrocodile.Services.Relay
{
    public interface IRelayService : IService
    {
        Task<Allocation> CreateAllocationAsync(int maxConnections, string region = null);
        Task<JoinAllocation> JoinAllocationAsync(string joinCode);
        Task<string> GetJoinCodeAsync(Guid allocationId);
    }
}
