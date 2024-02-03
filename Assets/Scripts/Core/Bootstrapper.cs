using BlindCrocodile.Core.Services;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using BlindCrocodile.GameStates;
using Unity.Netcode;

namespace BlindCrocodile.Core
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private LoaderWidget _loaderWidget;

        private static Game _game;

        private async void Awake()
        {
            await InitializeUnityServices();

            var sceneLoader = new SceneLoader(this);
            _game = new Game(sceneLoader, _loaderWidget, ServiceLocator.Instance, NetworkManager.Singleton, this);
            _game.GameStateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }

        private async Task InitializeUnityServices()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}