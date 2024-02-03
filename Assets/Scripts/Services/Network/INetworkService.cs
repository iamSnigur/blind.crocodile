using BlindCrocodile.Core.Services;
using System.Threading.Tasks;
using UnityEngine;

namespace BlindCrocodile.Services.Network
{
    public interface INetworkService : IService
    {
        public bool IsServer { get; }
        public bool IsClient { get; }
        public ulong ClientId { get; }

        Task StartHostAsync(int maxConnections);
        Task JoinServerAsync();
        void Disconnect();
        void AddNetworkPrefab(GameObject gameObject);
    }
}