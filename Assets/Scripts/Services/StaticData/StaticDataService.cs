using BlindCrocodile.StaticData;
using UnityEngine;

namespace BlindCrocodile.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private const string UI_PATH = "StaticData/UIStaticData";

        public UIStaticData UIStaticData { get; private set; }

        public void LoadUI() => 
            UIStaticData = Resources.Load<UIStaticData>(UI_PATH);
    }
}
