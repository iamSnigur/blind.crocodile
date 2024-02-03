using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace BlindCrocodile.Lobbies
{
    public class LocalLobby
    {
        private const string RELAY_KEY = "RelayCode";

        public event Action<LocalLobby> OnChanged;

        public string Id
        {
            get => _lobbyData.Id;

            set
            {
                if (!_lobbyData.Id.Equals(value))
                {
                    _lobbyData.Id = value;
                    NotifyListeners();
                }
            }
        }

        public string Name
        {
            get => _lobbyData.Name;

            set
            {
                if (!_lobbyData.Name.Equals(value))
                {
                    _lobbyData.Name = value;
                    NotifyListeners();
                }
            }
        }

        public string HostId
        {
            get => _lobbyData.HostId;

            set
            {
                if (!_lobbyData.HostId.Equals(value))
                {
                    _lobbyData.HostId = value;
                    NotifyListeners();
                }
            }
        }

        public string LobbyCode
        {
            get => _lobbyData.LobbyCode;

            set
            {
                if (!_lobbyData.LobbyCode.Equals(value))
                {
                    _lobbyData.LobbyCode = value;
                    NotifyListeners();
                }
            }
        }

        public string RelayCode
        {
            get => _lobbyData.RelayCode;

            set
            {
                if (!_lobbyData.RelayCode.Equals(value))
                {
                    _lobbyData.RelayCode = value;
                    NotifyListeners();
                }
            }
        }

        public int MaxPlayers
        {
            get => _lobbyData.MaxPlayers;

            set
            {
                if (!_lobbyData.MaxPlayers.Equals(value))
                {
                    _lobbyData.MaxPlayers = value;
                    NotifyListeners();
                }
            }
        }

        public int PlayersCount => Players.Count;

        public Dictionary<string, LocalPlayer> Players { get; private set; }

        private LobbyData _lobbyData;

        public LocalLobby() =>
            Players = new Dictionary<string, LocalPlayer>();

        public void Reset(LocalPlayer localPlayer)
        {
            LobbyData resetData = new()
            {
                Id = string.Empty,
                Name = string.Empty,
                HostId = string.Empty,
                LobbyCode = string.Empty,
                RelayCode = string.Empty,
                MaxPlayers = -1,
            };

            _lobbyData = resetData;

            Players.Clear();
            AddPlayer(localPlayer);
        }

        public void SetRemoteData(Lobby remoteLobby)
        {
            string relayCode = string.Empty;

            if (remoteLobby.Data != null)
                relayCode = remoteLobby.Data.TryGetValue(RELAY_KEY, out DataObject relayData)
                       ? relayData.Value
                       : string.Empty;

            LobbyData remoteData = new()
            {
                Id = remoteLobby.Id,
                Name = remoteLobby.Name,
                HostId = remoteLobby.HostId,
                LobbyCode = remoteLobby.LobbyCode,
                RelayCode = relayCode,
                MaxPlayers = remoteLobby.MaxPlayers,
            };

            _lobbyData = remoteData;

            List<LocalPlayer> playersFromRemote = new(remoteLobby.Players.Count);

            foreach (Player remote in remoteLobby.Players)
            {
                if (Players.ContainsKey(remote.Id))
                {
                    playersFromRemote.Add(Players[remote.Id]);
                    continue;
                }

                string name = "Player";
                ulong networkId = 0;

                if (remote.Data != null)
                {
                    name = remote.Data.TryGetValue("DisplayName", out PlayerDataObject nameObject)
                        ? nameObject.Value
                        : "Player";
                }

                LocalPlayer newLocal = new()
                {
                    NetworkId = networkId,
                    Id = remote.Id,
                    Name = name,
                    IsHost = HostId.Equals(remote.Id),
                };

                playersFromRemote.Add(newLocal);
            }

            List<LocalPlayer> playersToRemove = new();

            foreach (KeyValuePair<string, LocalPlayer> local in Players)
            {
                if (!playersFromRemote.Contains(local.Value))
                    playersToRemove.Add(local.Value);
            }

            playersToRemove.ForEach(p => RemovePlayerWithoutNotification(p));

            foreach (LocalPlayer fromRemote in playersFromRemote)
            {
                if (!Players.ContainsKey(fromRemote.Id))
                    AddPlayerWithoutNotification(fromRemote);
            }

            NotifyListeners();
        }

        public Dictionary<string, DataObject> GetDataForRemoteLobby() =>
            new()
            {
                [RELAY_KEY] = new DataObject(DataObject.VisibilityOptions.Member, RelayCode),
            };

        public void AddPlayer(LocalPlayer localPlayer)
        {
            if (Players.ContainsKey(localPlayer.Id))
                return;

            AddPlayerWithoutNotification(localPlayer);
            NotifyListeners();
        }

        public void RemovePlayer(LocalPlayer localPlayer)
        {
            if (!Players.ContainsKey(localPlayer.Id))
                return;

            RemovePlayerWithoutNotification(localPlayer);
            NotifyListeners();
        }

        private void AddPlayerWithoutNotification(LocalPlayer localPlayer)
        {
            Players.Add(localPlayer.Id, localPlayer);
            localPlayer.OnChanged += OnPlayerChanged;
        }

        private void RemovePlayerWithoutNotification(LocalPlayer localPlayer)
        {
            Players.Remove(localPlayer.Id);
            localPlayer.OnChanged -= OnPlayerChanged;
        }

        private void OnPlayerChanged(LocalPlayer player)
        {

        }

        private void NotifyListeners() =>
            OnChanged?.Invoke(this);
    }

    public struct LobbyData
    {
        public string Id;
        public string Name;
        public string HostId;
        public string LobbyCode;
        public string RelayCode;
        public int MaxPlayers;
    }
}