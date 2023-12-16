using BlindCrocodile.StateMachine;
using BlindCrocodile.Core;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using BlindCrocodile.Lobbies;

namespace BlindCrocodile.UI
{
    public class MenuHudController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _playerNameInput; // make fields validators
        [SerializeField] private TMP_InputField _joinCodeInput;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _exitButton;

        private IStateMachine _gameStateMachine;
        private ILobbyService _lobbyService;

        public void Construct(IStateMachine gameStateMachine, ILobbyService lobbyService)
        {
            _gameStateMachine = gameStateMachine;
            _lobbyService = lobbyService;

            _joinButton.onClick.AddListener(JoinGame);
            _hostButton.onClick.AddListener(HostGame);
            _exitButton.onClick.AddListener(Exit);
        }

        private void HostGame()
        {
            if (string.IsNullOrEmpty(_playerNameInput.text)) // refactor
                return;

            UpdateLocalPlayer(isHost: true);
            _gameStateMachine.Enter<HostGameState>();
        }

        private void JoinGame()
        {
            if (string.IsNullOrEmpty(_playerNameInput.text)
                || string.IsNullOrEmpty(_joinCodeInput.text))
                return;

            UpdateLocalPlayer();
            _gameStateMachine.Enter<JoinGameState, string>(_joinCodeInput.text);
        }

        private void Exit() =>
            Application.Quit();

        private void UpdateLocalPlayer(bool isHost = false)
        {
            _lobbyService.LocalPlayer.Name = _playerNameInput.text;
            _lobbyService.LocalPlayer.IsHost = isHost;
        }
    }
}