using System;
using System.Collections.Generic;
using CodeBase.Data;
using UnityEngine;
using UnityEngine.Purchasing;


namespace CodeBase.Infrastructure.Services.IAP
{
    public class IAPProvider : IStoreListener
    {
        private const string IAPConfigsPath = "IAP/products";
        
        private IStoreController _controller;
        private IExtensionProvider _extensions;
        
        private List<ProductConfig> _configs;

        public event Action Initialized;
        
        public bool IsInitialized => _controller != null && _extensions != null;


        public void Initialize() {
            Load();
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var productConfig in _configs) 
                builder.AddProduct(productConfig.Id, productConfig.Type);

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            _controller = controller;
            _extensions = extensions;

            Initialized?.Invoke();
            
            Debug.Log("UnityPurchasing initialization success ");
        }

        public void OnInitializeFailed(InitializationFailureReason error) {
            Debug.Log($"UnityPurchasing OnInitializeFailed: {error} ");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
            throw new System.NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
        }

        private void Load() {
            _configs = Resources.Load<TextAsset>(IAPConfigsPath).text.ToDeserialized<ProductConfigWrapper>().Configs;
        }
    }
}