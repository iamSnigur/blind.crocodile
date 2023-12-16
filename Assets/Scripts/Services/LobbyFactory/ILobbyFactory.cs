using BlindCrocodile.Core.Services;
using BlindCrocodile.Lobbies;
using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.Services.LobbyFactory
{
    public interface ILobbyFactory : IService
    {
        GameObject CreateHud();
        PlayerHudItem CreatePlayerItem(LocalPlayer localPlayer, Transform parent);
    }
}