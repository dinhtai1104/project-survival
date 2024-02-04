using com.foundation.iap.core;
using UnityEngine.Purchasing;

namespace com.foundation.iap.unity
{
    public static class UnityProductExtensions
    {
        public static ProductType ToUnityProductType(this PurchaseType type)
        {
            switch (type)
            {
                case PurchaseType.Consumable:
                    return ProductType.Consumable;
                case PurchaseType.NonConsumable:
                    return ProductType.NonConsumable;
                case PurchaseType.Subscription:
                    return ProductType.Subscription;
                default:
                    return ProductType.Consumable;
            }
        }

        public static PurchaseType ToLocalProductType(this ProductType type)
        {
            switch (type)
            {
                case ProductType.Consumable:
                    return PurchaseType.Consumable;
                case ProductType.NonConsumable:
                    return PurchaseType.NonConsumable;
                case ProductType.Subscription:
                    return PurchaseType.Subscription;
                default:
                    return PurchaseType.Consumable;
            }
        }
    }
}