using BlindCrocodile.Core.Services;
using BlindCrocodile.Lobbies;
using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.Services.LobbyFactory
{
    public interface ILobbyFactory : IService
    {
        LobbyHudController CreateHud();
        PlayerHudItem CreatePlayerItem(LocalPlayer localPlayer, Transform parent);
        ColorStatItem CreateColorStatItem(Color color, float amount, Transform parent);
    }
}