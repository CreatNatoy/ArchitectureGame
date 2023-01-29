using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.Ads;
using CodeBase.Infrastructure.Services.IAP;
using CodeBase.Infrastructure.Services.PersistentProgress;
using TMPro;
using UnityEngine;

namespace CodeBase.UI.Windows.Shop
{
    public class ShopWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _skullText;
        [SerializeField] private RewardedAdItem _adItem;
        [SerializeField] private ShopItemsContainer _shopItemsContainer;

        public void Construct(IAdsService adsService, IPersistentProgressService progressService,
            IIAPService iapService, IAssets assets) {
            base.Construct(progressService);
            _adItem.Construct(adsService, progressService);
            _shopItemsContainer.Construct(iapService, progressService, assets);
        }

        protected override void Initialize() {
            _adItem.Initialize();
            _shopItemsContainer.Initialize();
            RefreshSkullText();
        }

        protected override void SubscribeUpdates() {
            _adItem.Subscribe();
            _shopItemsContainer.Subscribe();
            Progress.WorldData.LootData.Changed += RefreshSkullText;
        }

        protected override void Cleanup() {
            base.Cleanup();
            _adItem.Cleanup();
            _shopItemsContainer.Cleanup();
            Progress.WorldData.LootData.Changed -= RefreshSkullText;
        }
        
        private void RefreshSkullText() => 
            _skullText.text = Progress.WorldData.LootData.Collected.ToString();
    }
}