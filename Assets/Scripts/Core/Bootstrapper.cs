using UnityEngine;

namespace BlindCrocodile.Core
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private Game _game;

        private void Awake()
        {            
            DontDestroyOnLoad(this);            
            _game = new Game();
            _game.StateMachine.Enter<BootstrapState>();
        }
    }
}