using BlindCrocodile.UI;
using UnityEngine;

namespace BlindCrocodile.StaticData
{
    [CreateAssetMenu(fileName = "UIStaticData", menuName = "BlindCrocodile/StaticData/UIStaticData")]
    public class UIStaticData : ScriptableObject
    {
        public GameObject MenuHudPrefab;
        public LobbyHudController LobbyHudPrefab;
        public PlayerHudItem PlayerHudItemPrefab;
        public ColorStatItem ColorStatItemPrefab;
    }
}