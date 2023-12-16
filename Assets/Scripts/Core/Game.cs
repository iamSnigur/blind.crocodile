using BlindCrocodile.Core.Services;

namespace BlindCrocodile.Core
{
    public class Game
    {
        public IStateMachine StateMachine { get; private set; }

        public Game(SceneLoader sceneLoader, LoaderWidget loaderWidget, ServicesContainer services, ICoroutineRunner coroutineRunner) =>
            StateMachine = new GameStateMachine(sceneLoader, loaderWidget, services, coroutineRunner);
    }
}