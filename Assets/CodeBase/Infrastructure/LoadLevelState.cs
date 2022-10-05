using System;
using CodeBase.CameraLogic;
using CodeBase.Logic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string InitialPoint = "InitialPoint";
        private const string HeroPath = "Hero/hero";
        private const string HudPath = "Hud/Hud";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;

        public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain) {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
        }

        public void Enter(string nameScene) { 
            _curtain.Show();
            _sceneLoader.Load(nameScene, OnLoaded);
        }

        public void Exit() => _curtain.Hide();

        private void OnLoaded() {
            var initialPoint = GameObject.FindWithTag(InitialPoint);
            
            var hero = Instantiate(HeroPath, initialPoint.transform.position);
            Instantiate(HudPath);
            CameraFollow(hero);
            
            _stateMachine.Enter<GameLoopState>();
        }

        private static GameObject Instantiate(string path) {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab); 
        }
        
        private static GameObject Instantiate(string path, Vector3 at) {
            var prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, at, Quaternion.identity); 
        }
        
        private static void CameraFollow(GameObject hero) => Camera.main.GetComponent<CameraFollow>().Follow(hero);

    }
}