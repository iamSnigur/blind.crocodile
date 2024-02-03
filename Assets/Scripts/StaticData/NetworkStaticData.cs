using BlindCrocodile.Services.Network;
using UnityEngine;

namespace BlindCrocodile.StaticData
{
    [CreateAssetMenu(fileName = "NetworkStaticData", menuName = "BlindCrocodile/StaticData/NetworkStaticData")]
    public class NetworkStaticData : ScriptableObject
    {
        public NetworkPlayersLobby NetworkPlayerPrefab;
    }
}