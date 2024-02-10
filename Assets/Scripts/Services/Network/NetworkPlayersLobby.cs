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
        public event Action<NetworkList<NetworkPlayer>> OnPlayerListChanged;
        public event Action<PlayerRole> OnGameStarted;
        public event Action<byte[]> OnArtistSharedCanvas;

        public NetworkList<NetworkPlayer> Players;
        public NetworkVariable<bool> IsLobbyClosed = new(false);

        private ILobbyService _lobbyService;
        private NetworkPlayer _localPlayer; // remove

        private void Awake()
        {
            Players = new();
            _lobbyService = ServiceLocator.Instance.Single<ILobbyService>();
            LocalPlayer localPlayer = _lobbyService.LocalPlayer;
            _localPlayer = new NetworkPlayer(NetworkManager.LocalClientId, localPlayer.Name, localPlayer.Id, false, PlayerRole.Guesser, Array.Empty<byte>().ToBytesContainer());
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            OnPlayerListChanged?.Invoke(Players);
            Players.OnListChanged += OnListChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Players.OnListChanged -= OnListChanged;
        }

        public void ShareCanvas(byte[] canvasBytes)
        {
            UpdatePlayerCanvasServerRpc(NetworkManager.LocalClientId, canvasBytes.ToBytesContainer());
        }

        public void ToggleReady()
        {
            _localPlayer.IsReady = !_localPlayer.IsReady;
            Debug.Log("ToggleReady client: " + _localPlayer);
            UpdatePlayerReadyServerRpc(NetworkManager.LocalClientId, _localPlayer.IsReady);
        }

        // update player list for new players // call from server
        public void StartGame()
        {
            for (int i = 0; i < Players.Count; i++)
                if (!Players[i].IsReady)
                {
                    Debug.Log("Not all players are ready");
                    return;
                }

            if (Players.Count <= 1)
            {
                Debug.Log("Not enought players");
                return;
            }

            // reset roles : for test
            for (int i = 0; i < Players.Count; i++)
                Players[i] = Players[i].SetRole(PlayerRole.Guesser);

            int artistIndex = UnityEngine.Random.Range(0, Players.Count);
            Players[artistIndex] = Players[artistIndex].SetRole(PlayerRole.Artist);

            NotifyGameStartClientRpc();            
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdatePlayerReadyServerRpc(ulong playerId, bool isReady)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Id.Equals(playerId))
                {
                    Players[i] = new NetworkPlayer(playerId, Players[i].Name, Players[i].LobbyId, isReady, Players[i].Role, Players[i].CanvasBytes);
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdatePlayerCanvasServerRpc(ulong playerId, BytesContainer canvasBytes)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Id.Equals(playerId))
                {
                    Players[i] = new NetworkPlayer(playerId, Players[i].Name, Players[i].LobbyId, Players[i].IsReady, Players[i].Role, canvasBytes);
                    // notify clients
                    NotifyCanvasSharedClientRpc(Players[i].CanvasBytes);
                    break;
                }
            }
        }

        [ClientRpc]
        public void NotifyCanvasSharedClientRpc(BytesContainer artistCanvas)
        {
            OnArtistSharedCanvas?.Invoke(artistCanvas.Bytes.ToArray());
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

            // invoke on game started
            OnGameStarted?.Invoke(networkPlayer.Role);
        }

        private void OnListChanged(NetworkListEvent<NetworkPlayer> changeEvent)
        {
            Debug.Log($"<b>Lobby id of the client {changeEvent.Value.Name}: {changeEvent.Value.LobbyId}</b>");
            OnPlayerListChanged?.Invoke(Players);
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
            new(player.Id, player.Name, player.LobbyId, isReady, player.Role, player.CanvasBytes);

        public static NetworkPlayer SetRole(this NetworkPlayer player, PlayerRole role) =>
           new(player.Id, player.Name, player.LobbyId, player.IsReady, role, player.CanvasBytes);

        public static BytesContainer ToBytesContainer(this byte[] bytes) =>
            new(bytes);
    }

    public struct NetworkPlayer : INetworkSerializable, IEquatable<NetworkPlayer>
    {
        public ulong Id;
        public FixedString32Bytes Name;
        public FixedString32Bytes LobbyId;
        public bool IsReady;
        public PlayerRole Role;
        public BytesContainer CanvasBytes;

        public NetworkPlayer(ulong id, string name, string lobbyId, bool isReady, PlayerRole role, BytesContainer canvasBytes)
        {
            Id = id;
            Name = new FixedString32Bytes(name);
            LobbyId = new FixedString32Bytes(lobbyId);
            IsReady = isReady;
            Role = role;
            CanvasBytes = canvasBytes;
        }

        public NetworkPlayer(ulong id, FixedString32Bytes name, FixedString32Bytes lobbyId, bool isReady, PlayerRole role, BytesContainer canvasBytes)
        {
            Id = id;
            Name = name;
            LobbyId = lobbyId;
            IsReady = isReady;
            Role = role;
            CanvasBytes = canvasBytes;
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
            serializer.SerializeValue(ref CanvasBytes);
        }
    }

    public struct BytesContainer : INetworkSerializable, IDisposable
    {
        public NativeArray<byte> Bytes;

        public BytesContainer(byte[] bytes)
        {
            Bytes = new(bytes, Allocator.Persistent);
        }

        public void Dispose() =>
            Bytes.Dispose();

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int length = 0;

            if (!serializer.IsReader)
                length = Bytes.Length;

            serializer.SerializeValue(ref length);

            if (serializer.IsReader)
            {
                if (Bytes.IsCreated)
                    Bytes.Dispose();

                Bytes = new NativeArray<byte>(length, Allocator.Persistent);
            }

            for (int n = 0; n < length; ++n)
            {
                byte val = Bytes[n];
                serializer.SerializeValue(ref val);
                Bytes[n] = val;
            }
        }
    }

    public enum PlayerRole : byte
    {
        Artist,
        Guesser
    }
}