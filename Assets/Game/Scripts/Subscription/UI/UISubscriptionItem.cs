using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using com.mec;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UISubscriptionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TitleStatusTxt;
    [SerializeField] private TextMeshProUGUI m_TitleNameTxt;
    [SerializeField] private TextMeshProUGUI m_DescriptionTxt;
    [SerializeField] private TextMeshProUGUI m_TimeEndTxt;

    [SerializeField] private Image m_IconImg;
    [SerializeField] private UIInventorySlot m_PurchaseRewardPanel;
    [SerializeField] private UIInventorySlot m_PurchaseRewardDailyPanel;

    [SerializeField] private Text m_PriceTxt;
    [SerializeField] private GameObject m_RemoveRewardAds;
    [SerializeField] private GameObject m_ActivatedGO;
    [SerializeField] private GameObject m_ButtonGO;


    [SerializeField] private TextMeshProUGUI m_DungeonCoinTxt;
    [SerializeField] private TextMeshProUGUI m_DungeonExpTxt;
    [SerializeField] private TextMeshProUGUI m_EnergyAddTxt;

    [SerializeField] private TextMeshProUGUI m_totalGemClaimInMonthTxt;

    private SubscriptionEntity entity;
    private SubscriptionSave save;

    public void Init(SubscriptionEntity entity, SubscriptionSave save)
    {
        this.entity = entity;
        this.save = save;

        SetInformation();
    }

    private void SetInformation()
    {
        m_TitleNameTxt.text = I2Localize.GetLocalize("Common/Title_Subscription_" + entity.Type);
        m_DescriptionTxt.text = I2Localize.GetLocalize("Common/Description_Subscription_" + entity.Type);
        m_TitleStatusTxt.text = I2Localize.GetLocalize("Common/Title_Subscription_Status_" + (save.IsActived ? 1 : 0));

        m_DungeonCoinTxt.text = I2Localize.GetLocalize("Common/Title_Subscription_DungeonCoin") + $" +{entity.ExtraGoldDungeon:0%}";
        m_DungeonExpTxt.text = I2Localize.GetLocalize("Common/Title_Subscription_DungeonExp") + $" +{entity.ExtraExpDungeon:0%}";
        m_EnergyAddTxt.text = I2Localize.GetLocalize("Common/Title_Subscription_DungeonEnergyAdd") + $" +{entity.Energy}";

        m_RemoveRewardAds.SetActive(entity.RemoveAdReward);

        m_totalGemClaimInMonthTxt.text = $"{(entity.Reward.Data.ValueLoot + 30 * entity.RewardDaily.Data.ValueLoot).TruncateValue()}";

        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(entity.ProductId);
        var price = IAPManager.Instance.GetPrice(entity.ProductId).PriceShow();
        m_PriceTxt.text = $"{price} {isoCode}";

        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams("Resource"), entity.Reward.Data, m_PurchaseRewardPanel.transform).ContinueWith(t=>
        {
            m_PurchaseRewardPanel.SetItem(t);
        }).Forget();

        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams("Resource"), entity.RewardDaily.Data, m_PurchaseRewardDailyPanel.transform).ContinueWith(t =>
        {
            m_PurchaseRewardDailyPanel.SetItem(t);
        }).Forget();

        m_ActivatedGO.SetActive(save.IsActived);
        m_ButtonGO.SetActive(!save.IsActived);
        m_TimeEndTxt.gameObject.SetActive(save.IsActived);
        Timing.KillCoroutines(gameObject);
        if (!save.IsActived)
        {

            return;
        }
        m_TitleStatusTxt.text = "";
        Timing.RunCoroutine(_Ticks());
    }

    private IEnumerator<float> _Ticks()
    {
        var end = save.TimeEnd;
        while(true)
        {
            var current = DateTime.UtcNow;
            var left = end - current;

            m_TimeEndTxt.text =/* I2Localize.GetLocalize("Common/Title_TimeEndIn") + " " + */left.ConvertTimeToString();
            if (left.TotalSeconds <= 0) break;
            yield return Timing.WaitForSeconds(1f);
        }
        save.IsActived = false;
        save.Deactive();
        save.Save?.Invoke();
        SetInformation();
    }

    public void BuyOnClicked()
    {
        IAPManager.Instance.BuyProduct(entity.ProductId, OnPurchaseComplete);
    }

    private async void OnPurchaseComplete(IAPManager.PurchaseState purchaseStatus, IAPPackage package)
    {
        if (purchaseStatus != IAPManager.PurchaseState.Success)
        {
            return;
        }
        save.Active();
        SetInformation();

        var subscription = Architecture.Get<SubscriptionService>();
        var ui = await PanelManager.ShowRewards(new List<LootParams>()
        {
            entity.Reward,
            entity.RewardDaily,
        });
        ui.SetTitle(I2Localize.GetLocalize("Common/Title_Common_Reward"));
        subscription.Reward();

        // Play Sound
        Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Product_Bought);
    }
}