using BlindCrocodile.Services.Network;
using BlindCrocodile.Services.StaticData;
using Unity.Netcode;
using UnityEngine;

namespace BlindCrocodile.Services.Factories
{
    public class NetworkFactory : INetworkFactory
    {
        public NetworkPlayersLobby NetworkPlayerView { get; private set; }

        private readonly IStaticDataService _staticDataService;
        private readonly INetworkService _networkService;

        public NetworkFactory(IStaticDataService staticDataService, INetworkService networkService)
        {
            _staticDataService = staticDataService;
            _networkService = networkService;
            _networkService.AddNetworkPrefab(_staticDataService.NetworkStaticData.NetworkPlayerPrefab.gameObject);
        }

        public NetworkPlayersLobby CreateNetworkPlayer()
        {
            NetworkPlayerView = Object.Instantiate(_staticDataService.NetworkStaticData.NetworkPlayerPrefab);

            if (_networkService.IsServer)
            {
                NetworkPlayerView
                .GetComponent<NetworkObject>()
                .Spawn();
            }

            return NetworkPlayerView;
        }
    }
}
