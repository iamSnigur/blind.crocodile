using BlindCrocodile.Services;

namespace BlindCrocodile.Core
{
    public class Game
    {
        public IStateMachine StateMachine { get ; private set; }

        public Game(SceneLoader sceneLoader, LoaderWidget loaderWidget, ServicesContainer services)
        {
            StateMachine = new GameStateMachine(sceneLoader, loaderWidget, services);
        }
    }
}