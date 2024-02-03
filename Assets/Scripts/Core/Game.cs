using BlindCrocodile.Core.Services;
using BlindCrocodile.GameStates;
using BlindCrocodile.NetworkStates;
using BlindCrocodile.GameplayStates;
using Unity.Netcode;

namespace BlindCrocodile.Core
{
    public class Game
    {
        public GameStateMachine GameStateMachine { get; private set; }
        public GameplayStateMachine GameplayStateMachine { get; private set; }
        public NetworkStateMachine NetworkStateMachine { get; private set; }

        public NetworkManager NetworkManager { get; private set; }

        public Game(SceneLoader sceneLoader, LoaderWidget loaderWidget, ServiceLocator services, NetworkManager networkManager, ICoroutineRunner coroutineRunner)
        {
            NetworkManager = networkManager;
            NetworkStateMachine = new NetworkStateMachine();
            GameStateMachine = new GameStateMachine(sceneLoader, loaderWidget, services, NetworkStateMachine, coroutineRunner);
            NetworkStateMachine.Construct(services, NetworkManager, loaderWidget);
        }
    }
}