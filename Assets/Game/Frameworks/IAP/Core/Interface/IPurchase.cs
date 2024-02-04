using System;
using System.Collections.Generic;

namespace com.foundation.iap.core
{
    public interface IPurchase
    {
        event Action<PurchaseInitialize> PurchaseInitialized;
        event Action<PurchaseError> PurchaseFailed;
        event Action<PurchaseComplete> PurchaseCompleted;
        event Action<PurchaseProcess> PurchaseProcessing;
        event Action<PurchaseComplete> ServerPurchaseValid;
        IPurchaseValidator LocalValidator { get; }
        IPurchaseValidator ServerValidator { get; }
        IEnumerable<IProduct> Products { get; }
        bool IsInitialized { get; }
        void InitializePurchasing(IEnumerable<ProductData> productData);
        void Purchase(string productId);
        void RestorePurchases();
    }
}