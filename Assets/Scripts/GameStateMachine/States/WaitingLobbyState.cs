using BlindCrocodile.Core;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.Lobbies;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace BlindCrocodile.GameStates
{
    public class WaitingLobbyState : IGameState, IState
    {
        private readonly AbstractStateMachine<IGameState> _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public WaitingLobbyState(AbstractStateMachine<IGameState> stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
        }

        public void Exit()
        {

        }
    }

    public class NetworkPlayers : NetworkBehaviour
    {
        private NetworkManager _networkManager;
        private ILobbyService _lobbyService;
        private NetworkList<NetworkPlayer> _players = new();

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            _networkManager.OnClientConnectedCallback += ClientConnected;
            _networkManager.OnClientDisconnectCallback += ClientDisconnected;
        }

        private void ClientDisconnected(ulong clientId)
        {
            for (int i = 0; i < _players.Count; i++)
                if (clientId == _players[i].Id)
                {
                    _players.RemoveAt(i);
                    return;
                }
        }

        private void ClientConnected(ulong clientId)
        {
            LocalPlayer localPlayer = _lobbyService.GetLocalPlayerByClientId(clientId);
            _players.Add(new NetworkPlayer(clientId, localPlayer.Name, false, PlayerRole.Guesser));
        }

        public struct NetworkPlayer : INetworkSerializable, IEquatable<NetworkPlayer>
        {
            public ulong Id;
            public FixedString32Bytes Name;
            public bool IsReady;
            public PlayerRole Role;

            public NetworkPlayer(ulong id, string name, bool isReady, PlayerRole role)
            {
                Id = id;
                Name = name;
                IsReady = isReady;
                Role = role;
            }

            public bool Equals(NetworkPlayer other) =>
                Id == other.Id
             && Name.Equals(other.Name)
             && IsReady == other.IsReady
             && Role == other.Role;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Id);
                serializer.SerializeValue(ref Name);
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
}
