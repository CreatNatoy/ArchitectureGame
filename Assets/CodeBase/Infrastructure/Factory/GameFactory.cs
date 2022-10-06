using System.Collections.Generic;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssets _assets;

        public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
        public List<ISavedProgress> ProgressesWriters { get; } = new List<ISavedProgress>();

        public GameFactory(IAssets asset) {
            _assets = asset; 
        }
        
        public GameObject CreateHero(GameObject at) => InstantiateRegistered(AssetPath.HeroPath, at.transform.position);

        public void CreateHud() => InstantiateRegistered(AssetPath.HudPath);

        public void Cleanup() {
            ProgressesWriters.Clear();
            ProgressReaders.Clear();
        }

        private GameObject InstantiateRegistered(string prefabPath, Vector3 at) {
            var gameObject = _assets.Instantiate(prefabPath, at);
            RegisterProgressWatchers(gameObject);
            return gameObject; 
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

        private void Register(ISavedProgressReader progressReader) {
            if(progressReader is ISavedProgress progressWriter)
                ProgressesWriters.Add(progressWriter);
            
            ProgressReaders.Add(progressReader);
        }
    }
}