using BlindCrocodile.Core.Services;
using BlindCrocodile.Core.StateMachine;
using BlindCrocodile.GameStates;

namespace BlindCrocodile.Core
{
    public class Game
    {
        public IStateMachine<IGameState> StateMachine { get; private set; }

        public Game(SceneLoader sceneLoader, LoaderWidget loaderWidget, ServiceLocator services, ICoroutineRunner coroutineRunner) =>
            StateMachine = new GameStateMachine(sceneLoader, loaderWidget, services, coroutineRunner);
    }
}