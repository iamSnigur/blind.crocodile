using UnityEngine.UI;
using UnityEngine;
using TMPro;
using BlindCrocodile.Lobbies;
using BlindCrocodile.GameStates;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.NetworkStates;

namespace BlindCrocodile.UI
{
    public class MenuHudController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _playerNameInput; // make fields validators
        [SerializeField] private TMP_InputField _joinCodeInput;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _exitButton;

        private AbstractStateMachine<IGameState> _gameStateMachine;
        private NetworkStateMachine _networkStateMachine;
        private ILobbyService _lobbyService;

        public void Construct(AbstractStateMachine<IGameState> gameStateMachine, ILobbyService lobbyService, NetworkStateMachine networkStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _lobbyService = lobbyService;
            _networkStateMachine = networkStateMachine;

            _joinButton.onClick.AddListener(JoinGame);
            _hostButton.onClick.AddListener(HostGame);
            _exitButton.onClick.AddListener(Exit);

            _playerNameInput.text = _lobbyService.LocalPlayer.Name;
        }

        private void HostGame()
        {
            if (string.IsNullOrEmpty(_playerNameInput.text)) // refactor
                return;

            UpdateLocalPlayer(isHost: true);
            _gameStateMachine.Enter<HostGameState>(); // enter load scene state
            //_networkStateMachine.Enter<HostState, int>(5);
        }

        private void JoinGame()
        {
            if (string.IsNullOrEmpty(_playerNameInput.text)
                || string.IsNullOrEmpty(_joinCodeInput.text))
                return;

            UpdateLocalPlayer();
            _gameStateMachine.Enter<JoinGameState, string>(_joinCodeInput.text); // same here
            //_networkStateMachine.Enter<JoinState, string>(_joinCodeInput.text);
        }

        // TODO refactor
        private void Exit() =>
            Application.Quit();

        private void UpdateLocalPlayer(bool isHost = false)
        {
            _lobbyService.LocalPlayer.Name = _playerNameInput.text;
            _lobbyService.LocalPlayer.IsHost = isHost;
        }
    }
}