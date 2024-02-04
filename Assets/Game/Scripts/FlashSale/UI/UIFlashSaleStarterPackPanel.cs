using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UI;
using UnityEngine;

public class UIFlashSaleStarterPackPanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private UIInventorySlot[] slots;

    private FlashSaleEntity entity;
    public override void PostInit()
    {
        entity = DataManager.Base.FlashSale.Get(EFlashSale.StarterPack);
    }

    public override async void Show()
    {
        base.Show();
        priceTxt.text = $"{IAPManager.Instance.GetPrice(entity.ProductId)} {IAPManager.Instance.GetIsoCurrencyCode(entity.ProductId)}";
        for (int i = 0; i < entity.Rewards.Count; i++)
        {
            var rw = entity.Rewards[i];
            var slot = slots[i];
            var path = AddressableName.UILootItemPath.AddParams(rw.Type);
            if (rw.Type == ELootType.Equipment)
            {
                path = AddressableName.UIGeneralEquipmentItem;
            }
            var icon = await UIHelper.GetUILootIcon(path, rw.Data, slot.transform);
            slot.SetItem(icon);
        }
    }

    private void OnClose()
    {
        var uifloat = ResourcesLoader.Instance.Get<UIFloatIcon>(AddressableName.UIFloatIcon, PanelManager.Instance.transform);
        var menu = PanelManager.Instance.GetPanel<UIMainMenuPanel>();
        var target = menu.FindFSButton(EFlashSale.StarterPack);
        target.GetComponent<CanvasGroup>().alpha = 0;
        target.SetActive(true);
        var featureButton = ResourcesLoader.Instance.GetSprite(AtlasName.FeatureButton, EFlashSale.StarterPack.ToString());
        uifloat.Set(featureButton, Vector3.zero, target.GetComponent<RectTransform>(), 0.6f, (t) =>
        {
            ResourcesLoader.Instance.GetAsync<ParticleSystem>("VFX_UIFloat_Icon", t.transform).ContinueWith(effect =>
            {
                effect.transform.localPosition = Vector3.zero;
                t.GetComponent<CanvasGroup>().alpha = 1;
                effect.Play();
            }).Forget();

            target.transform.DOScale(Vector3.one * 0.7f, 0.1f).SetEase(Ease.InSine).OnComplete(() =>
            {
                target.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnKill(() =>
                {
                    target.transform.localScale = Vector3.one;
                });
            }).OnKill(() =>
            {
                target.transform.localScale = Vector3.one;
            });
        });
        uifloat.Run().Forget();
    }


    public void BuyOnClicked()
    {
        IAPManager.Instance.BuyProduct(entity.ProductId, PurchaseComplete);
    }

    private void PurchaseComplete(IAPManager.PurchaseState status, IAPPackage package)
    {
        if (package.id == entity.ProductId)
        {
            if (status == IAPManager.PurchaseState.Success)
            {
                DataManager.Save.FlashSale.GetSave(EFlashSale.StarterPack).Claim();
                PanelManager.ShowRewards(entity.Rewards).Forget();
                Close();
            }
        }
    }
}
