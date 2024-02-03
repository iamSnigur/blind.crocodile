using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.GameStates;
using BlindCrocodile.Lobbies;
using BlindCrocodile.NetworkStates;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.Services.MenuFactory
{
    public class MenuFactory : IMenuFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly AbstractStateMachine<IGameState> _stateMachine;
        private readonly NetworkStateMachine _networkStateMachine;
        private readonly ILobbyService _lobbyService;

        public MenuFactory(IStaticDataService staticDataService, AbstractStateMachine<IGameState> stateMachine, ILobbyService lobbyService, NetworkStateMachine networkStateMachine)
        {
            _staticDataService = staticDataService;
            _stateMachine = stateMachine;
            _lobbyService = lobbyService;
            _networkStateMachine = networkStateMachine;
        }

        public GameObject CreateHud()
        {
            GameObject hud = Object.Instantiate(_staticDataService.UIStaticData.MenuHudPrefab);
            hud
                .GetComponent<MenuHudController>()
                .Construct(_stateMachine, _lobbyService, _networkStateMachine);

            return hud;
        }
    }

    public class UIFactory
    {

    }
}