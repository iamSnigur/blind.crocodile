using UnityEngine;

namespace BlindCrocodile.Services.LobbyFactory
{
    public interface ILobbyFactory : IService
    {
        GameObject CreateHub();
    }
}