using System;
using UnityEngine;

namespace Scripts.Core
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;

        public BootstrapState(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            // initialize frame rate
            //QualitySettings.vSyncCount = 0;
            //Application.targetFrameRate = 165;
            // warm up shaders
            //Shader.WarmupAllShaders();
            // initialize services
            InitializeServices();
        }

        public void Exit()
        {

        }

        private void InitializeServices()
        {

        }
    }
}