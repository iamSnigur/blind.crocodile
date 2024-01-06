using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.GameStates;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.Network;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.Services.LobbyFactory
{
    public class LobbyFactory : ILobbyFactory
    {
        private readonly INetworkService _networkService;
        private readonly IStaticDataService _staticDataService;
        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly ILobbyService _lobbyService;

        public LobbyFactory(INetworkService networkService, IStaticDataService staticDataService, IStateMachine<IGameState> stateMachine, ILobbyService lobbyService)
        {
            _networkService = networkService;
            _staticDataService = staticDataService;
            _stateMachine = stateMachine;
            _lobbyService = lobbyService;
        }

        public GameObject CreateHud()
        {
            GameObject hud = Object.Instantiate(_staticDataService.UIStaticData.LobbyHudPrefab);
            hud
                .GetComponent<LobbyHudController>()
                .Construct(_networkService, _stateMachine, _lobbyService, this);

            return hud;
        }

        public PlayerHudItem CreatePlayerItem(LocalPlayer localPlayer, Transform parent)
        {
            PlayerHudItem playerItem = 
                Object.Instantiate(_staticDataService.UIStaticData.PlayerHudItemPrefab, parent);
            playerItem.Construct(localPlayer);

            return playerItem;
        }
    }
}