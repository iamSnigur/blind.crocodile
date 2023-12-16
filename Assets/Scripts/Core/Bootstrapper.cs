using BlindCrocodile.Core.Services;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace BlindCrocodile.Core
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private LoaderWidget _loaderWidget;

        private Game _game;

        private async void Awake()
        {
            await InitializeUnityServices();

            var sceneLoader = new SceneLoader(this);
            _game = new Game(sceneLoader, _loaderWidget, ServicesContainer.Instance, this);
            _game.StateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }

        private async Task InitializeUnityServices()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}