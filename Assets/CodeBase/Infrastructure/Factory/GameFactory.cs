using System.Collections.Generic;
using CodeBase.Enemy;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using CodeBase.Logic.EnemySpawners;
using CodeBase.Services.Randomizer;
using CodeBase.StaticData;
using CodeBase.UI;
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

        public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
        public List<ISavedProgress> ProgressesWriters { get; } = new List<ISavedProgress>();
        private GameObject HeroGameObject { get; set; }

        public GameFactory(IAssets asset, IStaticDataService staticData, IRandomService randomService, IPersistentProgressService persistentProgressService) {
            _assets = asset;
            _staticData = staticData;
            _randomService = randomService;
            _progressService = persistentProgressService;
        }
        
        public GameObject CreateHero(GameObject at) {
            
            HeroGameObject = InstantiateRegistered(AssetPath.HeroPath, at.transform.position);
            return HeroGameObject; 
        }

        public GameObject CreateHud() {
            GameObject hud = InstantiateRegistered(AssetPath.HudPath);
            
            hud.GetComponentInChildren<LootCounter>().Construct(_progressService.Progress.WorldData);
            
            return hud;
        }

        public GameObject CreateMonster(MonsterTypeId typeId, Transform parent) {
            var monsterData = _staticData.ForMonster(typeId);
            var monster = Object.Instantiate(monsterData.Prefab, parent.position, Quaternion.identity, parent);

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

        public LootPiece CreateLoot() {
            var lootPiece = InstantiateRegistered(AssetPath.Loot).GetComponent<LootPiece>();
            
            lootPiece.Construct(_progressService.Progress.WorldData);
            
            return lootPiece;
        }

        public void Cleanup() {
            ProgressesWriters.Clear();
            ProgressReaders.Clear();
        }

        public void CreateSpawner(Vector3 at, string spawnerId, MonsterTypeId monsterTypeId) {
            SpawnPoint spawner = InstantiateRegistered(AssetPath.Spawner)
                .GetComponent<SpawnPoint>();
            
            spawner.Construct(this);
            spawner.Id = spawnerId;
            spawner.MonsterTypeId = monsterTypeId;
        }

        private GameObject InstantiateRegistered(string prefabPath, Vector3 at) {
            var gameObject = _assets.Instantiate(prefabPath, at);
            RegisterProgressWatchers(gameObject);
            return gameObject; 
        }

        public void Register(ISavedProgressReader progressReader) {
            if(progressReader is ISavedProgress progressWriter)
                ProgressesWriters.Add(progressWriter);
            
            ProgressReaders.Add(progressReader);
        }

        private GameObject InstantiateRegistered(string prefabPath) {
            var gameObject = _assets.Instantiate(prefabPath);
            RegisterProgressWatchers(gameObject);
            return gameObject; 
        }

        private void RegisterProgressWatchers(GameObject gameObject) {
            foreach (var progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
                Register(progressReader);
        }
    }
}