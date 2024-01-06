using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.GameStates;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.Services.MenuFactory
{
    public class MenuFactory : IMenuFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IStateMachine<IGameState> _stateMachine;
        private readonly ILobbyService _lobbyService;

        public MenuFactory(IStaticDataService staticDataService, IStateMachine<IGameState> stateMachine, ILobbyService lobbyService)
        {
            _staticDataService = staticDataService;
            _stateMachine = stateMachine;
            _lobbyService = lobbyService;
        }

        public GameObject CreateHud()
        {
            GameObject hud = Object.Instantiate(_staticDataService.UIStaticData.MenuHudPrefab);
            hud
                .GetComponent<MenuHudController>()
                .Construct(_stateMachine, _lobbyService);

            return hud;
        }
    }
}