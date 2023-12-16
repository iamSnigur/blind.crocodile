using BlindCrocodile.Core.Services;
using System.Threading.Tasks;

namespace BlindCrocodile.Services.Network
{
    public interface INetworkService : IService
    {
        Task StartHostAsync(int maxConnections);
        Task JoinServerAsync();
        void Disconnect();
    }
}