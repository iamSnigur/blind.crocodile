using BlindCrocodile.Core;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Network;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.UI
{
    public class LobbyHudController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playersCountLabel;
        [SerializeField] private TextMeshProUGUI _joinCodeLabel;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _readyButton; // if client
        [SerializeField] private Transform _playerHudItemsContainer;

        private readonly Dictionary<string, PlayerHudItem> _playerHudItems = new();
        private INetworkService _networkService;
        private IStateMachine _gameStateMachine;
        private ILobbyService _lobbyService;
        private ILobbyFactory _lobbyFactory;

        public void Construct(INetworkService networkService, IStateMachine gameStateMachine, ILobbyService lobbyService, ILobbyFactory lobbyFactory)
        {
            _networkService = networkService;
            _gameStateMachine = gameStateMachine;
            _lobbyService = lobbyService;
            _lobbyFactory = lobbyFactory;
            _exitButton.onClick.AddListener(Exit);
            _lobbyService.LocalLobby.OnChanged += OnLocalLobbyChanged;
        }

        private void OnDestroy() =>
            _lobbyService.LocalLobby.OnChanged -= OnLocalLobbyChanged;

        private void Exit()
        {
            _networkService.Disconnect();
            _gameStateMachine.Enter<LoadMenuState>();
        }

        private void OnLocalLobbyChanged(LocalLobby lobby)
        {
            UpdateLabels(lobby);
            UpdatePlayersList(lobby);
        }

        private void UpdateLabels(LocalLobby lobby)
        {
            _joinCodeLabel.text = lobby.LobbyCode;
            _playersCountLabel.text = $"Players {lobby.PlayersCount}/{lobby.MaxPlayers}";
        }

        private void UpdatePlayersList(LocalLobby lobby)
        {
            Dictionary<string, LocalPlayer> incomingPlayers = lobby.Players;
            List<string> itemsToRemove = new();

            foreach (KeyValuePair<string, PlayerHudItem> hudItem in _playerHudItems)
                if (!incomingPlayers.ContainsKey(hudItem.Key))
                    itemsToRemove.Add(hudItem.Key);

            foreach (string hudItemKey in itemsToRemove)
            {
                Destroy(_playerHudItems[hudItemKey].gameObject);
                _playerHudItems.Remove(hudItemKey);
            }

            foreach (KeyValuePair<string, LocalPlayer> player in incomingPlayers)
            {
                if (_playerHudItems.ContainsKey(player.Key))
                    _playerHudItems[player.Key].Construct(player.Value);
                else
                {
                    PlayerHudItem playerItem = _lobbyFactory.CreatePlayerItem(player.Value, _playerHudItemsContainer);
                    _playerHudItems.Add(player.Key, playerItem);
                }
            }
        }
    }
}
