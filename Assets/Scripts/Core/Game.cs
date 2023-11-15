namespace BlindCrocodile.Core
{
    public class Game
    {
        public IStateMachine StateMachine { get ; private set; }

        public Game()
        {
            StateMachine = new GameStateMachine();
        }
    }
}