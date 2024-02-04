using System;
using UnityEngine.Purchasing;

[System.Serializable]
public class IapConfigTable : DataTable<string, IAPPackage>
{
    public override void GetDatabase()
    {
        DB_IapConfig.ForEachEntity(e =>
        {
            var Product = e.Get<string>("Product");
            var PurchaseType = e.Get<string>("PurchaseType");
            var Price = e.Get<float>("Price");
            Enum.TryParse(PurchaseType, out ProductType type);

            var uProduct = new IAPPackage(Product, Price, type);
            Dictionary.Add(uProduct.id, uProduct);
        });
    }

    public IAPPackage FindPackage(string productId)
    {
        if (Dictionary.ContainsKey(productId))
            return Dictionary[productId];
        return null;
    }
}