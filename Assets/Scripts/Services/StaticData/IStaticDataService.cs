using BlindCrocodile.StaticData;

namespace BlindCrocodile.Services.StaticData
{
    public interface IStaticDataService : IService
    {
        public UIStaticData UIStaticData { get; }
        void LoadUI();
    }
}