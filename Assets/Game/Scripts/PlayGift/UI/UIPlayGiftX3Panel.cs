using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayGiftX3Panel : UI.Panel
{
    [SerializeField] private UIInventorySlot slotPrefab;
    [SerializeField] private UILootCollectionView lootView;
    [SerializeField] private Text salePriceTxt;
    [SerializeField] private Text priceTxt;

    private int sale = 70;
    private string ProductId;
    private List<LootParams> rewards = new List<LootParams>();
    public override void PostInit()
    {
    }

    public override void Show()
    {
        base.Show();
        var db = DataManager.Base.PlayGift;
        var save = DataManager.Save.PlayGift;

        var entity = db.Dictionary.Values.First();

        var price = IAPManager.Instance.GetPrice(entity.ProductId);
        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(entity.ProductId);
        ProductId = entity.ProductId;

        priceTxt.text = $"{(price * 100 / (100 - 80)).PriceShow()} {isoCode}";
        salePriceTxt.text = $"{price.PriceShow()} {isoCode}";

        rewards = new List<LootParams>();
        foreach (var model in db.Dictionary)
        {
            for (int i = 0; i < 2; i++)
            {
                rewards.Add(model.Value.Reward);
            }
        }
        rewards = rewards.MergeWithEquipment();
        rewards = rewards.OrderByDescending(t => t.Type).ToList();
        lootView.Show(new LootCollectionData(rewards), slotPrefab).Forget();
        onClosed += OnClosed;
    }

    private void OnClosed()
    {
        PoolManager.Instance.Clear(slotPrefab.gameObject);
    }

    public void BuyPurchaseOnClicked()
    {
        IAPManager.Instance.BuyProduct(ProductId, OnPurchaseComplete);
    }

    private void OnPurchaseComplete(IAPManager.PurchaseState status, IAPPackage package)
    {
        if (status == IAPManager.PurchaseState.Success)
        {
            PanelManager.ShowRewards(rewards).Forget();
            DataManager.Save.PlayGift.Purchase();
            Close();
            Messenger.Broadcast(EventKey.PurchaseX3Gift);
        }
    }
}
