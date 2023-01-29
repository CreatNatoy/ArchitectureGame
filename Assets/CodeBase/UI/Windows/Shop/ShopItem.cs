using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Windows.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private Button _buyItemButton;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _quantityText; 
        [SerializeField] private TextMeshProUGUI _availableItemsLeftText;
        [SerializeField] private Image _icon;
        
        private ProductDescription _productDescription;
        private IIAPService _iapService;
        private IAssets _assets;

        public void Construct(ProductDescription productDescription, IIAPService iapService, IAssets assets)
        {
            _productDescription = productDescription;
            _assets = assets;
            _iapService = iapService;
        }

        public async void Initialize()
        {
            _buyItemButton.onClick.AddListener(OnBuyItemClick);

            _priceText.text = _productDescription.Config.Price;
            _quantityText.text = _productDescription.Config.Quantity.ToString();
            _availableItemsLeftText.text = _productDescription.AvailablePurchasesLeft.ToString();
            _icon.sprite = await _assets.Load<Sprite>(_productDescription.Config.Icon);
        }

        private void OnBuyItemClick() => 
            _iapService.StartPurchase(_productDescription.Id);
    }
}