using Cysharp.Threading.Tasks;
using Spine;
using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UIFlashSaleDungeonOfferPanel : UI.Panel
{
    [SerializeField] private UIInventorySlot[] slotRewards;
    private OfferDungeonEntity entity;

    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Text salePriceTxt;
    [SerializeField] private Text priceTxt;
    [SerializeField] protected TextMeshProUGUI m_XValueTxt;
    public override void PostInit()
    {
    }
    public void Show(int dungeon)
    {
        base.Show();

        entity = DataManager.Base.OfferDungeon.Get(dungeon);
        titleTxt.text = I2Localize.GetLocalize($"Offer/Dungeon_Title_{entity.Id}");
        descriptionTxt.text = I2Localize.GetLocalize($"Offer/Dungeon_Description_{entity.Id}");

        m_XValueTxt.text = $"{entity.XValue}x";
        if (entity.XValue == 1)
        {
            m_XValueTxt.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            m_XValueTxt.transform.parent.gameObject.SetActive(true);
        }

        for (int i = 0; i < slotRewards.Length; i++)
        {
            var reward = entity.Rewards[i];
            var pathLoot = string.Format(AddressableName.UILootItemPath, reward.Type);
            if (reward.Type == ELootType.Equipment)
            {
                pathLoot = AddressableName.UIGeneralEquipmentItem;
            }
            var parent = slotRewards[i];

            UIHelper.GetUILootIcon(pathLoot, reward.Data, parent.Holder).ContinueWith(t =>
            {
                parent.SetItem(t);
            }).Forget();
        }

        var price = IAPManager.Instance.GetPrice(entity.ProductId);
        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(entity.ProductId);

        priceTxt.text = $"{(price * 100 / (100 - entity.SaleOff)).PriceShow()} {isoCode}";
        salePriceTxt.text = $"{price.PriceShow()} {isoCode}";
    }

    public void PurchaseOnClicked()
    {
        IAPManager.Instance.BuyProduct(entity.ProductId, OnPurchaseCompleted);
    }

    private void OnPurchaseCompleted(IAPManager.PurchaseState status, IAPPackage package)
    {
        if (package.id == entity.ProductId)
        {
            if (status == IAPManager.PurchaseState.Success)
            {
                PanelManager.ShowRewards(entity.Rewards).Forget();
                DataManager.Save.Offer.OfferDungeon.GetItem(entity.Id).Bought();
                Close();
                Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Product_Bought);

                foreach (var rw in entity.Rewards)
                {
                    // Track
                    FirebaseAnalysticController.Tracker.NewEvent("buy_resource")
                        .AddStringParam("item_category", rw.Type.ToString())
                        .AddStringParam("item_id", (rw.Data as ResourceData).Resource.ToString())
                        .AddStringParam("source", "dungeon_bundle")
                        .AddStringParam("source_id", entity.ProductId)
                        .AddDoubleParam("value", rw.Data.ValueLoot)
                        .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource((rw.Data as ResourceData).Resource))
                        .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn((rw.Data as ResourceData).Resource))
                        .Track();
                }
            }
        }
    }
}
