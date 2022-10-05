using CodeBase.Infrastructure.AssetManagement;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssets _assets;

        public GameFactory(IAssets asset) {
            _assets = asset; 
        }
        public GameObject CreateHero(GameObject at) => _assets.Instantiate(AssetPath.HeroPath, at.transform.position);

        public void CreateHud() => _assets.Instantiate(AssetPath.HudPath);
    }
}