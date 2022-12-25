using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Enemy;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using CodeBase.Logic.EnemySpawners;
using CodeBase.Services.Randomizer;
using CodeBase.StaticData;
using CodeBase.UI.Elements;
using CodeBase.UI.Services.Windows;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssets _assets;
        private readonly IStaticDataService _staticData;
        private readonly IRandomService _randomService;
        private readonly IPersistentProgressService _progressService;
        private IWindowService _windowService;

        public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
        public List<ISavedProgress> ProgressesWriters { get; } = new List<ISavedProgress>();
        private GameObject HeroGameObject { get; set; }

        public GameFactory(IAssets asset, IStaticDataService staticData, IRandomService randomService,
            IPersistentProgressService persistentProgressService, IWindowService windowService) {
            _assets = asset;
            _staticData = staticData;
            _randomService = randomService;
            _progressService = persistentProgressService;
            _windowService = windowService;
        }

        public async Task WarmUp() {
          await _assets.Load<GameObject>(AssetsAddress.Loot);
          await _assets.Load<GameObject>(AssetsAddress.Spawner);
        }
        
        public GameObject CreateHero(Vector3 at) {
            
            HeroGameObject = InstantiateRegistered(AssetsAddress.HeroPath, at);
            return HeroGameObject; 
        }

        public GameObject CreateHud() {
            GameObject hud = InstantiateRegistered(AssetsAddress.HudPath);
            
            hud.GetComponentInChildren<LootCounter>().Construct(_progressService.Progress.WorldData);

            foreach (var openWindowButton in hud.GetComponentsInChildren<OpenWindowButton>())
                openWindowButton.Construct(_windowService);

            return hud;
        }

        public async Task<GameObject> CreateMonster(MonsterTypeId typeId, Transform parent) {
            var monsterData = _staticData.ForMonster(typeId);

            GameObject prefab = await _assets.Load<GameObject>(monsterData.PrefabReference); 
            var monster = Object.Instantiate(prefab, parent.position, Quaternion.identity, parent);

            var health = monster.GetComponent<IHealth>();
            health.Current = monsterData.Hp;
            health.Max = monsterData.Hp;

            monster.GetComponent<ActorUI>().Construct(health); 
            monster.GetComponent<AgentMoveToHero>().Construct(HeroGameObject.transform);

            var lootSpawner = monster.GetComponentInChildren<LootSpawner>();
            lootSpawner.SetLoot(monsterData.MinLoot, monsterData.MaxLoot);
            lootSpawner.Construct(this, _randomService);

            var attack = monster.GetComponent<Attack>();
            attack.Construct(HeroGameObject.transform, monsterData.Damage, monsterData.Cleavage, monsterData.EffectiveDistance, monsterData.AttackCooldown);
            
            monster.GetComponent<RotateToHero>()?.Construct(HeroGameObject.transform);
            
            return monster; 
        }

        public async Task CreateSpawner(Vector3 at, string spawnerId, MonsterTypeId monsterTypeId) {
            var prefab = await _assets.Load<GameObject>(AssetsAddress.Spawner);
            SpawnPoint spawner = InstantiateRegistered(prefab, at)
                .GetComponent<SpawnPoint>();
            
            spawner.Construct(this);
            spawner.Id = spawnerId;
            spawner.MonsterTypeId = monsterTypeId;
        }

        public async Task<LootPiece> CreateLoot() {
            GameObject prefab = await _assets.Load<GameObject>(AssetsAddress.Loot);
            var lootPiece = InstantiateRegistered(prefab).GetComponent<LootPiece>();
            lootPiece.Construct(_progressService.Progress.WorldData);
            
            return lootPiece;
        }

        public void Cleanup() {
            ProgressesWriters.Clear();
            ProgressReaders.Clear();
            
            _assets.CleanUp();
        }

        private GameObject InstantiateRegistered(string prefabPath, Vector3 at) {
            var gameObject = _assets.Instantiate(prefabPath, at);
            RegisterProgressWatchers(gameObject);
            return gameObject; 
        } 
        
        private GameObject InstantiateRegistered(GameObject prefab, Vector3 at) {
            var gameObject = Object.Instantiate(prefab, at, Quaternion.identity);
            RegisterProgressWatchers(gameObject);
            return gameObject; 
        }

        private void Register(ISavedProgressReader progressReader) {
            if(progressReader is ISavedProgress progressWriter)
                ProgressesWriters.Add(progressWriter);
            
            ProgressReaders.Add(progressReader);
        }

        private GameObject InstantiateRegistered(string prefabPath) {
            var gameObject = _assets.Instantiate(prefabPath);
            RegisterProgressWatchers(gameObject);
            return gameObject; 
        }
        
        private GameObject InstantiateRegistered(GameObject prefab) {
            var gameObject = Object.Instantiate(prefab);
            RegisterProgressWatchers(gameObject);
            return gameObject; 
        }

        private void RegisterProgressWatchers(GameObject gameObject) {
            foreach (var progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
                Register(progressReader);
        }
    }
}