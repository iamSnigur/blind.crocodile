using BlindCrocodile.Core.Services;
using BlindCrocodile.StaticData;

namespace BlindCrocodile.Services.StaticData
{
    public interface IStaticDataService : IService
    {
        public UIStaticData UIStaticData { get; }
        public NetworkStaticData NetworkStaticData { get; }

        void Load();
    }
}