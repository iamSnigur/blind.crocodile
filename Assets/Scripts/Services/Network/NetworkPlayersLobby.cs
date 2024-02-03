using BlindCrocodile.Core.Services;
using BlindCrocodile.Lobbies;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace BlindCrocodile.Services.Network
{
    public class NetworkPlayersLobby : NetworkBehaviour
    {
        public event Action<string, NetworkPlayer> OnPlayerListChanged;

        public NetworkList<NetworkPlayer> Players = new();
        public NetworkVariable<bool> IsLobbyClosed = new();

        private ILobbyService _lobbyService;
        private NetworkPlayer _localPlayer; // do not use local player

        private void Awake()
        {
            Debug.Log("NetworkPlayersLobby-Awake");
            _lobbyService = ServiceLocator.Instance.Single<ILobbyService>();
            LocalPlayer localPlayer = _lobbyService.LocalPlayer;
            _localPlayer = new NetworkPlayer(NetworkManager.LocalClientId, localPlayer.Name, localPlayer.Id, false, PlayerRole.Guesser);
            Players.OnListChanged += OnListChanged;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("NetworkPlayersLobby-OnNetworkSpawn");
        }

        public void ToggleReady()
        {
            _localPlayer.IsReady = !_localPlayer.IsReady;
            Debug.Log("ToggleReady client: " + _localPlayer);
            UpdateDataServerRpc(NetworkManager.LocalClientId, _localPlayer);
        }

        // update player list for new players
        public void StartGame()
        {
            for (int i = 0; i < Players.Count; i++)
                if (!Players[i].IsReady)
                {
                    Debug.Log("Not all players are ready");
                    return;
                }

            int artistIndex = UnityEngine.Random.Range(0, Players.Count);
            Players[artistIndex] = Players[artistIndex].SetRole(PlayerRole.Artist);

            NotifyGameStartClientRpc();

            // reset roles : for test
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i] = Players[i].SetRole(PlayerRole.Guesser);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateDataServerRpc(ulong playerId, NetworkPlayer networkPlayer)
        {
            Debug.Log("<b>UpdateDataServerRpc</b> " + networkPlayer);

            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Id.Equals(playerId))
                {
                    Players[i] = new NetworkPlayer(playerId, Players[i].Name, Players[i].LobbyId, networkPlayer.IsReady, Players[i].Role);
                    break;
                }
            }
        }

        [ClientRpc]
        public void NotifyGameStartClientRpc()
        {
            // use gameplay state machine to change to PreRound state
            NetworkPlayer networkPlayer = GetLocalPlayer();
            string color = networkPlayer.Role == PlayerRole.Artist
                ? "green"
                : "red";

            Debug.Log($"<color={color}>{networkPlayer.Role}</color>");
        }

        private void OnListChanged(NetworkListEvent<NetworkPlayer> changeEvent)
        {
            Debug.Log($"<b>Lobby id of client {changeEvent.Value.Name}: {changeEvent.Value.LobbyId}</b>");
            OnPlayerListChanged?.Invoke(changeEvent.Value.LobbyId.ToString(), changeEvent.Value);
        }

        private NetworkPlayer GetLocalPlayer()
        {
            for (int i = 0; i < Players.Count; i++)
                if (Players[i].Id == NetworkManager.LocalClientId)
                    return Players[i];

            throw new KeyNotFoundException();
        }
    }

    public static class SessionData
    {
        public static Dictionary<ulong, string> ConnectedPlayerIds = new();

        public static void AddPlayer(ulong networkId, string lobbyId) =>
            ConnectedPlayerIds.Add(networkId, lobbyId);

        public static void ClearPlayers() =>
            ConnectedPlayerIds.Clear();
    }

    public static class NetworkPlayerExtensions
    {
        public static NetworkPlayer SetIsReady(this NetworkPlayer player, bool isReady) =>
            new(player.Id, player.Name, player.LobbyId, isReady, player.Role);

        public static NetworkPlayer SetRole(this NetworkPlayer player, PlayerRole role) =>
           new(player.Id, player.Name, player.LobbyId, player.IsReady, role);
    }

    public struct NetworkPlayer : INetworkSerializable, IEquatable<NetworkPlayer>
    {
        public ulong Id;
        public FixedString32Bytes Name;
        public FixedString32Bytes LobbyId;
        public bool IsReady;
        public PlayerRole Role;

        public NetworkPlayer(ulong id, string name, string lobbyId, bool isReady, PlayerRole role)
        {
            Id = id;
            Name = new FixedString32Bytes(name);
            LobbyId = new FixedString32Bytes(lobbyId);
            IsReady = isReady;
            Role = role;
        }

        public NetworkPlayer(ulong id, FixedString32Bytes name, FixedString32Bytes lobbyId, bool isReady, PlayerRole role)
        {
            Id = id;
            Name = name;
            LobbyId = lobbyId;
            IsReady = isReady;
            Role = role;
        }

        public bool Equals(NetworkPlayer other) =>
            Id == other.Id
         && Name.Equals(other.Name)
         && LobbyId.Equals(other.LobbyId)
         && IsReady == other.IsReady
         && Role == other.Role;

        public override readonly string ToString() =>
            $"{Name} {Role} {IsReady} NetworkId: {Id} | LobbyId: {LobbyId}";

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref LobbyId);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Role);
        }
    }

    public enum PlayerRole : byte
    {
        Artist,
        Guesser
    }
}