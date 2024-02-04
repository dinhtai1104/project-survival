using System;
using System.Collections.Generic;

namespace com.foundation.iap.core
{
    public abstract class PurchaseDecorator : IPurchase
    {
        protected IPurchase Purchaser { get; }
        public event Action<PurchaseInitialize> PurchaseInitialized;
        public event Action<PurchaseError> PurchaseFailed;
        public event Action<PurchaseComplete> PurchaseCompleted;
        public event Action<PurchaseProcess> PurchaseProcessing;
        public event Action<PurchaseComplete> ServerPurchaseValid;
        public IPurchaseValidator LocalValidator => Purchaser.LocalValidator;
        public IPurchaseValidator ServerValidator => Purchaser.ServerValidator;
        public IEnumerable<IProduct> Products => Purchaser.Products;
        public bool IsInitialized => Purchaser.IsInitialized;

        protected PurchaseDecorator(IPurchase purchaser)
        {
            Purchaser = purchaser;
            Purchaser.PurchaseInitialized += PurchaseInitializedHandler;
            Purchaser.PurchaseFailed += PurchaseFailedHandler;
            Purchaser.PurchaseCompleted += PurchaseCompletedHandler;
            Purchaser.PurchaseProcessing += PurchaseProcessingHandler;
            Purchaser.ServerPurchaseValid += ServerPurchaseValidHandler;
        }

        protected virtual void PurchaseInitializedHandler(PurchaseInitialize purchaseInitialize)
        {
            PurchaseInitialized?.Invoke(purchaseInitialize);
        }

        protected virtual void PurchaseFailedHandler(PurchaseError purchaseError)
        {
            PurchaseFailed?.Invoke(purchaseError);
        }

        protected virtual void PurchaseProcessingHandler(PurchaseProcess purchaseProcess)
        {
            PurchaseProcessing?.Invoke(purchaseProcess);
        }

        protected virtual void PurchaseCompletedHandler(PurchaseComplete purchaseComplete)
        {
            PurchaseCompleted?.Invoke(purchaseComplete);
        }

        protected virtual void ServerPurchaseValidHandler(PurchaseComplete purchaseComplete)
        {
            ServerPurchaseValid?.Invoke(purchaseComplete);
        }

        public virtual void InitializePurchasing(IEnumerable<ProductData> productData)
        {
            Purchaser.InitializePurchasing(productData);
        }

        public virtual void Purchase(string productId)
        {
            Purchaser.Purchase(productId);
        }

        public virtual void RestorePurchases()
        {
            Purchaser.RestorePurchases();
        }
    }
}