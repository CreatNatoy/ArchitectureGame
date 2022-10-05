using System;
using CodeBase.CameraLogic;
using UnityEngine;
using Object = UnityEngine.Object;

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

        public void Enter(string nameScene) => _sceneLoader.Load(nameScene, OnLoaded);

        public void Exit() {
            throw new NotImplementedException();
        }

        private void OnLoaded() {
            var hero = Instantiate("Hero/hero");
            Instantiate("Hud/Hud");
            CameraFollow(hero);
        }

        private static GameObject Instantiate(string path) {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab); 
        }
        
        private static void CameraFollow(GameObject hero) => Camera.main.GetComponent<CameraFollow>().Follow(hero);

    }
}