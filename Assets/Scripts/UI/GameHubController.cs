using BlindCrocodile.Services.Multiplayer;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameHubController : MonoBehaviour // rename
{
    [SerializeField] private TMP_InputField _joinCodeInput;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _exitButton;

    private IMultiplayerService _multiplayerService;

    public void Construct(IMultiplayerService multiplayerService)
    {
        _multiplayerService = multiplayerService;
        _joinButton.onClick.AddListener(JoinGame);
        _hostButton.onClick.AddListener(HostGame);
        _exitButton.onClick.AddListener(Exit);
    }

    private void JoinGame() => 
        _multiplayerService.JoinServer(_joinCodeInput.text);

    private async void HostGame() => 
        await _multiplayerService.StartHost(4);

    private void Exit() => 
        Application.Quit();
}
