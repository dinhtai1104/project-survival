using com.foundation.iap.core;
using com.foundation.iap.mockup;
using com.foundation.iap.unity;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPPackage
{
    public string id;
    public float price;
    public ProductType type;

    public IAPPackage(string id, float price, ProductType type)
    {
        this.id = id;
        this.price = price;
        this.type = type;
    }
}

public class IAPManager : LiveSingleton<IAPManager>
{
    public enum PurchaseState
    {
        Success,
        Fail
    }

    private IPurchaseValidator Validator;
    private IPurchase Purchase;
    public UnityAction<PurchaseState, IAPPackage> OnPurchaseComplete;
    public UnityAction<PurchaseState, IAPPackage> OnPurchaseEvent;

    [SerializeField] private List<ProductData> productData;
    private List<IAPPackage> m_Products = new List<IAPPackage>();

    public void Init(List<IAPPackage> catalog)
    {
        productData = new List<ProductData>();
        foreach (var model in catalog)
        {
            m_Products.Add(model);
            switch (model.type)
            {
                case UnityEngine.Purchasing.ProductType.Consumable:
                    productData.Add(new ProductData(model.id, PurchaseType.Consumable, model.price.ToString()));
                    break;
                case UnityEngine.Purchasing.ProductType.NonConsumable:
                    productData.Add(new ProductData(model.id, PurchaseType.NonConsumable, model.price.ToString()));
                    break;
                case UnityEngine.Purchasing.ProductType.Subscription:
                    productData.Add(new ProductData(model.id, PurchaseType.Subscription, model.price.ToString()));
                    break;
            }
        }

#if IAP_PRODUCTION
        // validator
        Validator = new UnityPurchaseValidator(GooglePlayTangle.Data(), AppleTangle.Data());

        // iap
        Purchase = new UnityPurchase(Validator);
#elif IAP_DEVELOPMENT
        // validator
        Validator = new MockupPurchaseValidator();

        // iap
        Purchase = new MockupPurchase();
#endif

        Purchase.InitializePurchasing(productData);

        Purchase.PurchaseCompleted += OnPurchaseCompleted;
        Purchase.PurchaseFailed += OnPurchaseFailed;
    }
    public bool IsInitialized()
    {
        return Purchase != null && Purchase.IsInitialized;
    }

    public void BuyProduct(string productId, UnityAction<PurchaseState, IAPPackage> onPurchaseComplete)
    {
        Debug.Log(this + "Buy Process Started for " + productId);

        OnPurchaseComplete = onPurchaseComplete;

        for (var i = 0; i < productData.Count; i++)
        {
            if (productData[i].Id.Equals(productId))
            {
                BuyProductWithId(productData[i].Id);
            }
        }
    }
    private void BuyProductWithId(string productId)
    {
        Debug.Log(this + "Buy product with id: " + productId);

        if (IsInitialized())
        {
            WaitingPanel.Show();
            Purchase.Purchase(productId);
        }
        else
        {
            Debug.Log(this + "BuyProductID FAIL. Store not initialized.");

            if (OnPurchaseComplete != null)
            {
                OnPurchaseComplete(PurchaseState.Fail, m_Products.Find(t => t.id == productId));
            }
        }
    }

    private void OnPurchaseFailed(PurchaseError args)
    {
        WaitingPanel.Hide();
        Debug.Log(this + "Buy Product failed for " + args.ProductId + " Failed. Reason: " + args.ErrorType);
        OnPurchaseComplete?.Invoke(PurchaseState.Fail, m_Products.Find(t => t.id == args.ProductId));
        OnPurchaseEvent?.Invoke(PurchaseState.Fail, m_Products.Find(t=>t.id == args.ProductId));

    }

    private void OnPurchaseCompleted(PurchaseComplete args)
    {
        WaitingPanel.Hide();
        Debug.Log($"[{nameof(IAPManager)}] OnPurchaseCompleted. {args.ProductId}");
        Debug.Log($"[{nameof(IAPManager)}] OnPurchaseCompleted. {args.Receipts}");

        var product = m_Products.FirstOrDefault(cond => string.Equals(cond.id, args.ProductId));
        if (product != null)
        {
            OnPurchaseComplete?.Invoke(PurchaseState.Success, product);
            OnPurchaseEvent?.Invoke(PurchaseState.Success, product);
        }

        // Get Receipts
        var newProduct = Purchase.Products.FirstOrDefault(x => x.Id.Equals(args.ProductId));
        if (newProduct != null)
        {
            var responseData = (Dictionary<string, object>)MiniJson.JsonDecode(args.Receipts);
            if (responseData != null)
            {
                // tracking response data
                GameSceneManager.Instance.PrintLogInProduction(args.Receipts);
            }
        }
    }
    public decimal GetPrice(string productId)
    {
        try
        {

#if IAP_PRODUCTION
            if (IsInitialized())
            {
                return Purchase.Products.FirstOrDefault(x => x.Id.Equals(productId))!.LocalizedPrice;
            }

            Debug.LogError("Not Initialized");
            return decimal.Parse(productData.First(p => string.Equals(p.Id, productId)).Price.Replace("$", string.Empty));
#else
        return decimal.Parse(productData.First(p => string.Equals(p.Id, productId)).Price.Replace("$", string.Empty));
#endif
        }catch(System.Exception e)
        {
            Logger.LogError(e);
            return 0;
        }
    }
    public string GetIsoCurrencyCode(string productId)
    {
        IProduct product;
#if PRODUCTION
        try
        {
            if (IsInitialized())
            {
                product = Purchase.Products.FirstOrDefault(x => x.Id.Equals(productId));
                if (product != null)
                {
                    if (CurrencyTools.TryGetCurrencySymbol(product.IsoCurrencyCode, out string symbol))
                    {
                        return symbol;
                    }
                }
                if (product != null)
                {
                    return product.IsoCurrencyCode;
                }
            }
        }
        catch (System.Exception e)
        {
            if (IsInitialized())
            {
                product = Purchase.Products.FirstOrDefault(x => x.Id.Equals(productId));
                if (product != null)
                {
                    if (CurrencyTools.TryGetCurrencySymbol(product.IsoCurrencyCode, out string symbol))
                    {
                        return symbol;
                    }
                    return product.IsoCurrencyCode;
                }
            }
        }

        Debug.LogError("Not Initialized");
        return "$";
#elif IAP_DEVELOPMENT
        product = Purchase.Products.FirstOrDefault(x => x.Id.Equals(productId));
        if (product != null)
        {
            return product.IsoCurrencyCode;
        }
        return "$";
#endif
    }
}
