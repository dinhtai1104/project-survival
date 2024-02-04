using UnityEngine;
using com.foundation.iap.core;
using UnityEngine.Purchasing.Security;

namespace com.foundation.iap.mockup
{
    public class MockupPurchaseValidator : IPurchaseValidator
    {
        public PurchaseState Validate(string productId, string productType, string receipt)
        {
            return PurchaseState.Purchased;
        }
    }
}