using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace BlindCrocodile.Lobbies
{
    public class LocalPlayer
    {
        public event Action<LocalPlayer> OnChanged;

        public string Id;
        public string Name;
        public bool IsHost;

        public void Reset() =>
            IsHost = false;

        public Dictionary<string, PlayerDataObject> GetDataForRemoteLobby() =>
            new()
            {
                ["DisplayName"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, Name),
            };
    }

    public struct PlayerData
    {
        public string Id;
        public string Name;
        public bool IsHost;
    }
}