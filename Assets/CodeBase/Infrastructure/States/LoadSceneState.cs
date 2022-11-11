using CodeBase.CameraLogic;
using CodeBase.Hero;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using CodeBase.StaticData;
using CodeBase.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure.States
{
    public class LoadSceneState : IPayloadedState<string>
    {
        private const string InitialPoint = "InitialPoint";
        private const string EnemySpawnerTag = "EnemySpawner";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly IStaticDataService _staticData;

        public LoadSceneState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain,  IGameFactory gameFactory, IPersistentProgressService progressService, IStaticDataService staticDataService) {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
            _staticData = staticDataService; 
        }

        public void Enter(string nameScene) { 
            _curtain.Show();
            _gameFactory.Cleanup();
            _sceneLoader.Load(nameScene, OnLoaded);
        }

        public void Exit() => _curtain.Hide();

        private void OnLoaded() {
            InitGameWorld();
            InformProgressReaders(); 
            
            _stateMachine.Enter<GameLoopState>();
        }

        private void InformProgressReaders() => 
            _gameFactory.ProgressReaders.ForEach(x => x.LoadProgress(_progressService.Progress));

        private void InitGameWorld() {
            InitSpawners();
            
            var hero = InitHero();

            InitHud(hero);
            CameraFollow(hero);
        }       

        private void InitSpawners() {
            string sceneKey = SceneManager.GetActiveScene().name;
            LevelStaticData levelData = _staticData.ForLevel(sceneKey);

            foreach (EnemySpawnerData spawnerData in levelData.EnemySpawners) {
                _gameFactory.CreateSpawner(spawnerData.Position, spawnerData.Id, spawnerData.MonsterTypeId); 
            }
        }

        private GameObject InitHero() {
            return _gameFactory.CreateHero(at: GameObject.FindWithTag(InitialPoint));
        }

        private void InitHud(GameObject hero) {
            var hud = _gameFactory.CreateHud();
            
            hud.GetComponentInChildren<ActorUI>().Construct(hero.GetComponent<HeroHealth>());
        }

        private static void CameraFollow(GameObject hero) => Camera.main.GetComponent<CameraFollow>().Follow(hero);

    }
}