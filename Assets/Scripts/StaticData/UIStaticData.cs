using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.StaticData
{
    [CreateAssetMenu(fileName = "UIStaticData", menuName = "BlindCrocodile/StaticData/UIStaticData")]
    public class UIStaticData : ScriptableObject
    {
        public GameObject MenuHudPrefab;
        public GameObject LobbyHudPrefab;
        public PlayerHudItem PlayerHudItemPrefab;
    }
}
