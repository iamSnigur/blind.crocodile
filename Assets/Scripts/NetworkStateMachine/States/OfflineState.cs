using BlindCrocodile.Core.StateMachine;
using UnityEngine;

namespace BlindCrocodile.NetworkStates
{
    public class OfflineState : INetworkState, IState
    {
        public void Enter()
        {
            Debug.Log("OfflineState");
        }

        public void Exit()
        {
        }
    }
}