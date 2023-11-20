using BlindCrocodile.Services.Multiplayer;
using BlindCrocodile.Services.StaticData;
using UnityEngine;

namespace BlindCrocodile.Services.LobbyFactory
{
    public class LobbyFactory : ILobbyFactory
    {
        private readonly IMultiplayerService _multiplayerService;
        private readonly IStaticDataService _staticDataService;

        public LobbyFactory(IMultiplayerService multiplayerService, IStaticDataService staticDataService)
        {
            _multiplayerService = multiplayerService;
            _staticDataService = staticDataService;
        }

        public GameObject CreateHub()
        {
            GameObject hub = Object.Instantiate(_staticDataService.UIStaticData.LobbyHubPrefab);
            hub
                .GetComponent<GameHubController>()
                .Construct(_multiplayerService);

            return hub;
        }
    }
}