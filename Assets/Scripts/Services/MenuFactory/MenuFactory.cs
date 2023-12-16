using BlindCrocodile.Core;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.StaticData;
using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.Services.MenyFactory
{
    public class MenuFactory : IMenuFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IStateMachine _stateMachine;
        private readonly ILobbyService _lobbyService;

        public MenuFactory(IStaticDataService staticDataService, IStateMachine stateMachine, ILobbyService lobbyService)
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