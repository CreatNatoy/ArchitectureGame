using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.Ads;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.Services.Input;
using CodeBase.Services.Randomizer;
using CodeBase.StaticData;
using CodeBase.UI.Services.Factory;
using CodeBase.UI.Services.Windows;
using UnityEngine.Device;

namespace CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private AllServices _services;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services) {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader; 
            _services = services;
            
            RegisterServices();
        }

        public void Enter() {
            _sceneLoader.Load(Initial, onLoad: EnterLoadLevel);
        }

        public void Exit() {
            
        }

        private void EnterLoadLevel() => _stateMachine.Enter<LoadProgressState>();

        private void RegisterServices() {
            RegisterStaticData();
            RegisterAdsService();

            IRandomService randomService = new RandomService(); 
            
            _services.RegisterSingle(randomService);
            _services.RegisterSingle<IInputService>(InputService());
            _services.RegisterSingle<IGameStateMachine>(_stateMachine);
            RegisterAssetProvider();
            _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
            _services.RegisterSingle<IUIFactory>(new UIFactory(_services.Single<IAssets>(), _services.Single<IStaticDataService>(), 
                _services.Single<IPersistentProgressService>(), _services.Single<IAdsService>()));
            _services.RegisterSingle<IWindowService>(new WindowService(_services.Single<IUIFactory>()));
            _services.RegisterSingle<IGameFactory>(new GameFactory(_services.Single<IAssets>(), _services.Single<IStaticDataService>(), 
                randomService, _services.Single<IPersistentProgressService>(), _services.Single<IWindowService>()));
            _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(_services.Single<IPersistentProgressService>(), _services.Single<IGameFactory>()));
            
        }

        private void RegisterAssetProvider() {
            var assetProvider = new AssetProvider();
            _services.RegisterSingle<IAssets>(assetProvider);
            assetProvider.Initialize();
        }

        private void RegisterAdsService() {
            var adsService = new AdsService();
            adsService.Initialize();
            _services.RegisterSingle<IAdsService>(adsService);
        }

        private void RegisterStaticData() {
            IStaticDataService staticData = new StaticDataService();
            staticData.Load();
            _services.RegisterSingle(staticData);
        }

        private static IInputService InputService() {
            if (Application.isEditor)
                return new StandaloneInputService();
            
            return new MobileInputService();
        }
    }
}