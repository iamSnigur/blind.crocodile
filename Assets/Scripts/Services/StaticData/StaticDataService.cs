using BlindCrocodile.StaticData;
using UnityEngine;

namespace BlindCrocodile.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private const string UI_DATA_PATH = "StaticData/UIStaticData";
        private const string NETWORK_DATA_PATH = "StaticData/NetworkStaticData";

        public UIStaticData UIStaticData { get; private set; }
        public NetworkStaticData NetworkStaticData { get; private set; }

        public void Load()
        {
            UIStaticData = Resources.Load<UIStaticData>(UI_DATA_PATH);
            NetworkStaticData = Resources.Load<NetworkStaticData>(NETWORK_DATA_PATH);
        }
    }
}
