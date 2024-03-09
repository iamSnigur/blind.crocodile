using BlindCrocodile.GameStates;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.Factories;
using BlindCrocodile.Services.Network;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.Services.LobbyFactory
{
    public class LobbyFactory : ILobbyFactory
    {
        private readonly GameStateMachine _stateMachine;
        private readonly INetworkService _networkService;
        private readonly IStaticDataService _staticDataService;
        private readonly ILobbyService _lobbyService;
        private readonly INetworkFactory _networkFactory;

        public LobbyFactory(INetworkService networkService, IStaticDataService staticDataService, GameStateMachine stateMachine, ILobbyService lobbyService, INetworkFactory networkFactory)
        {
            _networkService = networkService;
            _staticDataService = staticDataService;
            _stateMachine = stateMachine;
            _lobbyService = lobbyService;
            _networkFactory = networkFactory;
        }

        public ColorStatItem CreateColorStatItem(Color color, float amount, Transform parent)
        {
            ColorStatItem statItem = Object.Instantiate(_staticDataService.UIStaticData.ColorStatItemPrefab, parent);
            statItem.Construct(color, amount);

            return statItem;
        }

        public LobbyHudController CreateHud()
        {
            LobbyHudController hud = Object.Instantiate(_staticDataService.UIStaticData.LobbyHudPrefab);

            hud.Construct(_networkService, _stateMachine, _lobbyService, this, _networkFactory);

            return hud;
        }

        public PlayerHudItem CreatePlayerItem(LocalPlayer localPlayer, Transform parent)
        {
            PlayerHudItem playerItem = Object.Instantiate(_staticDataService.UIStaticData.PlayerHudItemPrefab, parent);
            playerItem.Construct(localPlayer);

            return playerItem;
        }
    }
}