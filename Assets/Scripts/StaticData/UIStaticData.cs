using UnityEngine;

namespace BlindCrocodile.StaticData
{
    [CreateAssetMenu(fileName = "UIStaticData", menuName = "BlindCrocodile/StaticData/UIStaticData", order = 0)]
    public class UIStaticData : ScriptableObject
    {
        public GameObject LobbyHubPrefab;
    }
}
