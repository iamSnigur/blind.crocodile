using BlindCrocodile.Core.Services;
using System.Threading.Tasks;

namespace BlindCrocodile.Lobbies
{
    public interface ILobbyService : IService
    {
        LocalLobby LocalLobby { get; }
        LocalPlayer LocalPlayer { get; }

        Task CreateLobbyAsync(string lobbyName, int maxConnections);
        Task JoinLobbyByCodeAsync(string lobbyCode);
        void DisconnectFromLobby();
        Task UpdateRemoteLobbyDataAsync();
        Task UpdateRemotePlayerDataAsync(string allocationId, string relayConnectionInfo);
    }
}