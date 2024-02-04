using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIShopOfferGemItem : UIOfferBaseItem
{
    [SerializeField] private Text m_PriceTxt;
    private OfferGemEntity gemEntity => base.entity as OfferGemEntity;

    protected override void Setup()
    {
        base.Setup();
        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(gemEntity.ProductId);
        var price = IAPManager.Instance.GetPrice(gemEntity.ProductId).PriceShow();
        m_PriceTxt.text = $"{price} {isoCode}";
        m_Icon.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Offer, $"gem_{gemEntity.Id + 1}");
        m_Icon.SetNativeSize();
    }

    protected override void OnClickOffer()
    {
        IAPManager.Instance.BuyProduct(gemEntity.ProductId, OnCompletePurchased);
    }


    private void Purchased()
    {
        save.Bought();
        ShowRewards();
        Setup();
        Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Product_Bought);

        // Track
        FirebaseAnalysticController.Tracker.NewEvent("buy_resource")
            .AddStringParam("item_category", "Resource")
            .AddStringParam("item_id", EResource.Gem.ToString())
            .AddStringParam("source", "shop_gem")
            .AddStringParam("source_id", gemEntity.ProductId)
            .AddDoubleParam("value", gemEntity.GetValue().Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(EResource.Gem))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(EResource.Gem))
            .Track();
    }

    private void OnCompletePurchased(IAPManager.PurchaseState status, IAPPackage product)
    {
        if (status == IAPManager.PurchaseState.Success)
        {
            if (product.id == gemEntity.ProductId)
            {
                Purchased();
            }
        }
    }
}