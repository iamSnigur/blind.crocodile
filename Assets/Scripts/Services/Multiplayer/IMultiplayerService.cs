using System.Threading.Tasks;

namespace BlindCrocodile.Services.Multiplayer
{
    public interface IMultiplayerService : IService
    {
        Task<string> StartHost(int maxConnections);
        void JoinServer(string joinCode);
        void Disconnect();
    }
}