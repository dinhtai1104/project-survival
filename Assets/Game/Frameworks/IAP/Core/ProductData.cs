using System;
using UnityEngine;

namespace com.foundation.iap.core
{
    [Serializable]
    public struct ProductData
    {
        [SerializeField] private string id;
        [SerializeField] private PurchaseType type;
        [SerializeField] private string price;

        public string Id => id;

        public PurchaseType Type => type;

        public string Price => price;

        public ProductData(string id, PurchaseType type, string price)
        {
            this.id = id;
            this.type = type;
            this.price = price;
        }
    }
}