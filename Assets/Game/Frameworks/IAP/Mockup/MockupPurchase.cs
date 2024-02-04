using System;
using System.Collections.Generic;
using com.foundation.iap.core;
using com.foundation.iap.mockup;
// using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace com.foundation.iap.mockup
{
    public class MockupPurchase : IPurchase
    {
        public event Action<PurchaseInitialize> PurchaseInitialized;
        public event Action<PurchaseError> PurchaseFailed;
        public event Action<PurchaseComplete> PurchaseCompleted;
        public event Action<PurchaseProcess> PurchaseProcessing;
        public event Action<PurchaseComplete> ServerPurchaseValid;
        public IPurchaseValidator LocalValidator { private set; get; }
        public IPurchaseValidator ServerValidator { private set; get; }

        public IEnumerable<IProduct> Products => _products;
        public bool IsInitialized { private set; get; }

        private readonly IPurchaseValidator _validator;
        private readonly List<IProduct> _products;

        public MockupPurchase()
        {
            _validator = new MockupPurchaseValidator();
            _products = new List<IProduct>();
        }

        public void InitializePurchasing(IEnumerable<ProductData> productData)
        {
            foreach (var product in productData)
            {
                _products.Add(new MockupProduct(product));
            }

            IsInitialized = true;
            LocalValidator = null;
            ServerValidator = null;
            PurchaseInitialized?.Invoke(new PurchaseInitialize(InitializationStatus.Ready));
        }

        public void Purchase(string productId)
        {
            if (IsValidated(productId))
            {
                var purchase = new PurchaseComplete(productId, string.Empty);
                PurchaseCompleted?.Invoke(purchase);
            }
        }

        public void RestorePurchases() { }

        private bool IsValidated(string productId)
        {
            return _validator.Validate(productId, string.Empty, string.Empty) == PurchaseState.Purchased;
        }
    }
}