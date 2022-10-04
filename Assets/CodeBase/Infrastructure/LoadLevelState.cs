using System;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader) {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter(string nameScene) => SceneManager.LoadScene(nameScene);

        public void Exit() {
            throw new NotImplementedException();
        }
    }
}