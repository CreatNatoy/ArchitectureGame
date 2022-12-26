using System.Threading.Tasks;
using CodeBase.CameraLogic;
using CodeBase.Hero;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using CodeBase.StaticData;
using CodeBase.UI;
using CodeBase.UI.Elements;
using CodeBase.UI.Services.Factory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string EnemySpawnerTag = "EnemySpawner";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly IStaticDataService _staticData;
        private readonly IUIFactory _uiFactory;

        public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain,
            IGameFactory gameFactory, IPersistentProgressService progressService, IStaticDataService staticDataService,
            IUIFactory uiFactory) {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
            _staticData = staticDataService;
            _uiFactory = uiFactory;
        }

        public void Enter(string nameScene) { 
            _curtain.Show();
            _gameFactory.Cleanup();
            _gameFactory.WarmUp();
            _sceneLoader.Load(nameScene, OnLoaded);
        }

        public void Exit() => _curtain.Hide();

        private async void OnLoaded() {
            await InitUIRoot();
            await InitGameWorld();
            InformProgressReaders();
            
            _stateMachine.Enter<GameLoopState>();
        }

        private async Task InitUIRoot() => 
            await _uiFactory.CreateUIRoot();

        private void InformProgressReaders() => 
            _gameFactory.ProgressReaders.ForEach(x => x.LoadProgress(_progressService.Progress));

        private async Task InitGameWorld() {
            var levelData = LevelStaticData();

            await InitSpawners(levelData);
            var hero = await InitHero(levelData);

            await InitHud(hero);
            CameraFollow(hero);
        }

        private async Task InitSpawners(LevelStaticData levelData) {
            foreach (EnemySpawnerData spawnerData in levelData.EnemySpawners) {
              await _gameFactory.CreateSpawner(spawnerData.Position, spawnerData.Id, spawnerData.MonsterTypeId); 
            }
        }

        private async Task<GameObject> InitHero(LevelStaticData levelData) => 
           await _gameFactory.CreateHero(levelData.InitialHeroPosition);

        private async Task InitHud(GameObject hero) {
            var hud = await _gameFactory.CreateHud();
            
            hud.GetComponentInChildren<ActorUI>().Construct(hero.GetComponent<HeroHealth>());
        }

        private LevelStaticData LevelStaticData() =>
            _staticData.ForLevel(SceneManager.GetActiveScene().name);

            private static void CameraFollow(GameObject hero) => Camera.main.GetComponent<CameraFollow>().Follow(hero);
    }
}