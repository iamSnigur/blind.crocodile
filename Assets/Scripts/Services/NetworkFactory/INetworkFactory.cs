using BlindCrocodile.Core.Services;
using BlindCrocodile.Services.Network;

namespace BlindCrocodile.Services.Factories
{
    public interface INetworkFactory : IService
    {
        public NetworkPlayersLobby NetworkPlayerView { get; }

        NetworkPlayersLobby CreateNetworkPlayer();
    }
}