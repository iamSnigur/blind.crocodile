using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace BlindCrocodile.Lobbies
{
    public class LocalPlayer
    {
        public event Action<LocalPlayer> OnChanged;

        public ulong NetworkId;

        public string Id
        {
            get => _playerData.Id;

            set
            {
                if (!value.Equals(_playerData.Id))
                {
                    _playerData.Id = value;
                    NotifyListeners();
                }
            }
        }

        public string Name
        {
            get => _playerData.Name;

            set
            {
                if (!value.Equals(_playerData.Name))
                {
                    _playerData.Name = value;
                    NotifyListeners();
                }
            }
        }

        public bool IsHost
        {
            get => _playerData.IsHost;

            set
            {
                if (!value.Equals(_playerData.IsHost))
                {
                    _playerData.IsHost = value;
                    NotifyListeners();
                }
            }
        }

        private PlayerData _playerData;

        public void Reset()
        {
            _playerData = new PlayerData
            {
                Id = Id,
                Name = Name,
                IsHost = false,
            };
        }

        public Dictionary<string, PlayerDataObject> GetDataForRemoteLobby() =>
            new()
            {
                ["DisplayName"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, Name),
                ["NetworkId"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, NetworkId.ToString()),
            };

        private void NotifyListeners() =>
            OnChanged?.Invoke(this);

        public override string ToString()
        {
            return $"{Name} [{Id}] IsHost: {IsHost}";
        }
    }

    public struct PlayerData
    {
        public string Id;
        public string Name;
        public bool IsHost;
    }
}