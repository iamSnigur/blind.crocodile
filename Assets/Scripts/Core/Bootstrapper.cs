using UnityEngine;

namespace Scripts.Core
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private Game _game;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            IStateMachine stateMachine = new StateMachine();
            _game = new Game(stateMachine);
        }
    }
}