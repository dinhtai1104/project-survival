using com.foundation.iap.core;
using UnityEngine;
using UnityEngine.Purchasing;

namespace com.foundation.iap.unity
{
    public class UnityProduct : IProduct
    {
        public string Id => _product.definition.id;
        public PurchaseType Type => _product.definition.type.ToLocalProductType();
        public decimal LocalizedPrice => _product.metadata.localizedPrice;
        public string IsoCurrencyCode => _product.metadata.isoCurrencyCode;
        public string LocalizedPriceString => _product.metadata.localizedPriceString;
        public string LocalizedTitle => _product.metadata.localizedTitle;
        public string LocalizedDescription => _product.metadata.localizedDescription;
        public bool HasReceipt => _product.hasReceipt;
        public string Receipt => _product.receipt;

        private readonly Product _product;

        public UnityProduct(Product product)
        {
            _product = product;
            Debug.Log(ToString());
        }

        public sealed override string ToString()
        {
            return
                $"--------------------------------\n Id: {Id}\n Type: {Type}\n LocalizedPrice: {LocalizedPrice}\n IsoCurrencyCode: {IsoCurrencyCode}\n LocalizedPriceString: {LocalizedPriceString}\n --------------------------------\n";
        }
    }
}