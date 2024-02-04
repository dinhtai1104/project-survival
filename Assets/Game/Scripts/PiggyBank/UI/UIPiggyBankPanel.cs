using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.PiggyBank.Data;
using com.mec;
using Cysharp.Threading.Tasks;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPiggyBankPanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI piggyBankTimeReset;
    [SerializeField] private TextMeshProUGUI piggyBankLabel;
    [SerializeField] private TextMeshProUGUI progressPigCoinTxt;
    [SerializeField] private Image progressPigCoinFilled;

    [SerializeField] private UIInventorySlot freeSlotReward;
    [SerializeField] private UIInventorySlot adSlotReward;
    [SerializeField] private UIInventorySlot purchaseSlotReward;
    [SerializeField] private Text purchasePrice;

    [SerializeField] private GameObject freePanel, adPanel, purchasePanel;
    [SerializeField] private GameObject freeButtonClaimed, adButtonClaimed, purchaseButtonClaimed;
    [SerializeField] private GameObject freeButton, adButton, purchaseButton;


    private PiggyBankTable table;
    private PiggyBankServices piggyBankService;
    private ResourcesSave resource;
    private PiggyBankEntity entity;
    public override void PostInit()
    {
        piggyBankService = Architecture.Get<PiggyBankServices>();
        table = piggyBankService.Table;
        resource = DataManager.Save.Resources;
    }
    public override void Show()
    {
        base.Show();
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        entity = piggyBankService.GetTable(piggyBankService.PiggyCurrent);

        freePanel.SetActive(true);
        adPanel.SetActive(true);
        purchasePanel.SetActive(true);

        piggyBankLabel.text = I2Localize.GetLocalize("Common/Title_PiggyBank") + " " + piggyBankService.PiggyCurrent;
      

        progressPigCoinTxt.text = $"{resource.GetResource(EResource.PigCoin).TruncateValue()}/{entity.Target.TruncateValue()}";
        progressPigCoinFilled.fillAmount = (float)(resource.GetResource(EResource.PigCoin) * 1.0f / entity.Target);

        freeButtonClaimed.SetActive(piggyBankService.IsClaimCurrent(EPiggyBank.FREE));
        freeButton.SetActive(!piggyBankService.IsClaimCurrent(EPiggyBank.FREE));

        adButtonClaimed.SetActive(piggyBankService.IsClaimCurrent(EPiggyBank.AD));
        adButton.SetActive(!piggyBankService.IsClaimCurrent(EPiggyBank.AD));

        purchaseButtonClaimed.SetActive(piggyBankService.IsClaimCurrent(EPiggyBank.PURCHASE));
        purchaseButton.SetActive(!piggyBankService.IsClaimCurrent(EPiggyBank.PURCHASE));

        var address = entity.BaseValue.GetAllData()[0].Type.ToString();
        address = string.Format(AddressableName.UILootItemPath, address);
        UIHelper.GetUILootIcon(address, entity.BaseValue, freeSlotReward.transform)
            .ContinueWith(t =>
            {
                freeSlotReward.SetItem(t);
            }).Forget();
        UIHelper.GetUILootIcon(address, entity.AdValue, adSlotReward.transform)
            .ContinueWith(t =>
            {
                adSlotReward.SetItem(t);
            }).Forget();
        UIHelper.GetUILootIcon(address, entity.PurchaseValue, purchaseSlotReward.transform)
            .ContinueWith(t =>
            {
                purchaseSlotReward.SetItem(t);
            }).Forget();
        var productId = entity.ProductId;
        var price = IAPManager.Instance.GetPrice(productId).PriceShow();
        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(productId);
        purchasePrice.text = $"{price} {isoCode}";

        Timing.KillCoroutines(gameObject);
        Timing.RunCoroutine(_Ticks(), Segment.RealtimeUpdate, gameObject);
    }

    private IEnumerator<float> _Ticks()
    {
        while (true)
        {
            var left = piggyBankService.TimeEnd - UnbiasedTime.UtcNow;
            piggyBankTimeReset.text = I2Localize.GetLocalize("Common/Title_ResetIn", left.ConvertTimeToString());
            if (left.TotalSeconds <= 0)
            {   
                break;
            }
            yield return Timing.WaitForSeconds(1f);
        }

        piggyBankService.Active(piggyBankService.PiggyCurrent, true);
        UpdateUI();
    }

    public override void Close()
    {
        Timing.KillCoroutines(gameObject);
        base.Close();
    }

    private void OnClaimPiggyBank(EPiggyBank type)
    {
        switch (type)
        {
            case EPiggyBank.FREE:
                PanelManager.ShowRewards(entity.BaseValue.GetAllData()).Forget();
                break;
            case EPiggyBank.AD:
                PanelManager.ShowRewards(entity.AdValue.GetAllData()).Forget();
                break;
            case EPiggyBank.PURCHASE:
                PanelManager.ShowRewards(entity.PurchaseValue.GetAllData()).Forget();
                break;
        }
        piggyBankService.ClaimPiggy(type);
        UpdateUI();
    }

    public void ClaimFreeOnClicked()
    {
        if (!resource.HasResource(EResource.PigCoin, entity.Target))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, EResource.PigCoin.GetLocalize())).Forget();
            return;
        }
        OnClaimPiggyBank(EPiggyBank.FREE);
    }
    public void ClaimADOnClicked()
    {
        if (!resource.HasResource(EResource.PigCoin, entity.Target))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, EResource.PigCoin.GetLocalize())).Forget();
            return;
        }
        Architecture.Get<AdService>().ShowRewardedAd("piggy_bank_ads", (result) =>
        {
            if (result)
            {
                OnClaimPiggyBank(EPiggyBank.AD);
            }
            else
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.NotAd"));
            }
        }, placement: AD.AdPlacementKey.PIGGY_BANK);
    }
    public void ClaimPurchaseOnClicked()
    {
        if (!resource.HasResource(EResource.PigCoin, entity.Target))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, EResource.PigCoin.GetLocalize())).Forget();
            return;
        }
        IAPManager.Instance.BuyProduct(entity.ProductId, OnPurchaseComplete);
    }

    private void OnPurchaseComplete(IAPManager.PurchaseState state, IAPPackage package)
    {
        if (state == IAPManager.PurchaseState.Success)
        {
            OnClaimPiggyBank(EPiggyBank.PURCHASE);
            Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Product_Bought);
        }
    }
}
