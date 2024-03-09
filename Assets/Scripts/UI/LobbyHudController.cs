using BlindCrocodile.Gameplay.Drawing;
using BlindCrocodile.GameStates;
using BlindCrocodile.Lobbies;
using BlindCrocodile.Services.Factories;
using BlindCrocodile.Services.LobbyFactory;
using BlindCrocodile.Services.Network;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using NetworkPlayer = BlindCrocodile.Services.Network.NetworkPlayer;

namespace BlindCrocodile.UI
{
    public class LobbyHudController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playersCountLabel;
        [SerializeField] private TextMeshProUGUI _joinCodeLabel;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private TextMeshProUGUI _readyButtonLabel;
        [SerializeField] private Transform _playerHudItemsContainer;
        [SerializeField] private Color _readyColor;
        [SerializeField] private Color _notReadyColor;
        [SerializeField] private GameObject _lobbyModal;
        [SerializeField] private GameObject _artistModal;
        [SerializeField] private GameObject _guesserModal;
        [SerializeField] private GuesserHudController _guesserHudController;
        [SerializeField] private ArtistHudController _artistHudController;
        [SerializeField] private GameObject _uiBlocker;

        private readonly Dictionary<string, PlayerHudItem> _playerHudItems = new();
        private GameStateMachine _gameStateMachine;
        private INetworkService _networkService;
        private ILobbyService _lobbyService;
        private ILobbyFactory _lobbyFactory;
        private INetworkFactory _networkFactory;

        public void Construct(INetworkService networkService, GameStateMachine gameStateMachine, ILobbyService lobbyService, ILobbyFactory lobbyFactory, INetworkFactory networkFactory)
        {
            _networkService = networkService;
            _gameStateMachine = gameStateMachine;
            _lobbyService = lobbyService;
            _lobbyFactory = lobbyFactory;
            _networkFactory = networkFactory;
            _exitButton.onClick.AddListener(Exit);
            _readyButton.onClick.AddListener(Ready);
            _artistHudController.OnCanvasShared += OnCanvasShared;
            _lobbyService.LocalLobby.OnChanged += OnLocalLobbyChanged;

            _lobbyModal.SetActive(true);
            _artistModal.SetActive(false);
            _guesserModal.SetActive(false);

            _networkFactory.NetworkPlayerView.OnPlayerListChanged += OnPlayerListChanged;
            _networkFactory.NetworkPlayerView.OnGameStarted += OnGameStarted;
            _networkFactory.NetworkPlayerView.OnArtistSharedCanvas += OnArtistSharedCanvas;
        }

        private void OnCanvasShared(byte[] canvasBytes) => // when artist share his canvas
            _networkFactory.NetworkPlayerView.ShareCanvas(canvasBytes);

        private void OnArtistSharedCanvas(byte[] canvasBytes) // when guesser recieved shared canvas from an artist
        {
            _guesserHudController.Construct(_lobbyFactory, canvasBytes);
            _uiBlocker.SetActive(false);
        }

        private void OnPlayerListChanged(NetworkList<NetworkPlayer> networkPlayers)
        {
            foreach (NetworkPlayer player in networkPlayers)
            {
                string lobbyId = player.LobbyId.ToString();

                if (_playerHudItems.ContainsKey(lobbyId))
                    _playerHudItems[lobbyId].Construct(player);
            }
        }

        private void OnGameStarted(PlayerRole playerRole)
        {
            _lobbyModal.SetActive(false);

            switch (playerRole)
            {
                case PlayerRole.Artist:
                    _networkFactory.NetworkPlayerView.OnArtistSharedCanvas -= OnArtistSharedCanvas;
                    _artistModal.SetActive(true);
                    break;
                case PlayerRole.Guesser:
                    _uiBlocker.SetActive(true);
                    _guesserModal.SetActive(true);
                    break;
            }
        }

        private void OnDestroy()
        {
            _lobbyService.LocalLobby.OnChanged -= OnLocalLobbyChanged;
            _networkFactory.NetworkPlayerView.OnPlayerListChanged -= OnPlayerListChanged;
            _networkFactory.NetworkPlayerView.OnGameStarted -= OnGameStarted;
            _networkFactory.NetworkPlayerView.OnArtistSharedCanvas -= OnArtistSharedCanvas;
        }

        private void Exit()
        {
            _networkService.Disconnect();
            _gameStateMachine.Enter<LoadMenuState>();
        }

        private void Ready()
        {
            if (_networkService.IsServer)
                _networkFactory.NetworkPlayerView.StartGame();
            else
                _networkFactory.NetworkPlayerView.ToggleReady();
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
