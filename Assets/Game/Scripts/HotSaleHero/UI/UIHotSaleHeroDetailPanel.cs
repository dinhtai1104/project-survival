using com.foundation.iap.core;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIHotSaleHeroDetailPanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI m_NameHeroTxt;
    [SerializeField] private TextMeshProUGUI m_NamePassiveTxt;
    [SerializeField] private TextMeshProUGUI m_DescriptionPassiveTxt;

    [SerializeField] private Text m_SalePriceTxt;
    [SerializeField] private Text m_PriceTxt;

    [Header("Reward")]
    [SerializeField] private UIInventorySlot[] m_SlotsReward;

    [Header("Hero Gacha")]
    [SerializeField] private RectTransform m_HeroPos;
    private UIActor actor;

    private HotSaleHeroEntity entity;
    private HotSaleHeroSaves saves;

    public override void PostInit()
    {
    }

    [Button]
    public void Show(EHero hero)
    {
        base.Show();
        saves = DataManager.Save.HotSaleHero;
        entity = DataManager.Base.HotSaleHero.GetSaleHero(hero);

        m_NameHeroTxt.text = I2Localize.GetLocalize($"Hero_Name/{hero}");
        m_NamePassiveTxt.text = I2Localize.GetLocalize($"Buff_Name/{hero}Passive");
        m_DescriptionPassiveTxt.text = I2Localize.GetLocalize($"Buff_Description/{hero}Passive");

        var priceSale = IAPManager.Instance.GetPrice(entity.ProductId);
        var price = (priceSale * 100 / (100 - entity.SaleOff)).PriceShow();
        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(entity.ProductId);
        m_SalePriceTxt.text = $"{priceSale.PriceShow()} {isoCode}";
        m_PriceTxt.text = $"{price} {isoCode}";

        var ePath = string.Format(AddressableName.UIHero, hero);
        ResourcesLoader.Instance.GetAsync<UIActor>(ePath, m_HeroPos).ContinueWith(t =>
        {
            actor = t;
            actor.transform.localPosition = Vector3.zero;
            actor.SetAnimation(0, "jump/down", true);
        });
        SetupRewards();
    }

    public void SetJumpDownHero()
    {
        if (actor != null)
        {
            actor.SetAnimation(0, "jump/grounding", false);
            actor.AddAnimation(0, "idle/normal", true);
        }
    }

    private void SetupRewards()
    {
        var rewards = entity.Rewards;

        for (int i = 0; i < m_SlotsReward.Length; i++)
        {
            var rw = rewards[i];
            var path = string.Format(AddressableName.UILootItemPath, rw.Type);
            var parent = m_SlotsReward[i];
            UIHelper.GetUILootIcon(path, rw.Data, parent.transform, size: 1.3f).ContinueWith(icon =>
            {
                if (rw.Type == ELootType.Hero)
                {
                    icon.SetSizeImage(0.85f);
                }
                parent.SetItem(icon);
            }).Forget();
            
        }
    }

    public override void Close()
    {
        onClosed += () =>
        {
            if (actor != null)
            {
                var rewards = entity.Rewards;

                for (int i = 0; i < m_SlotsReward.Length; i++)
                {
                    var rw = rewards[i];
                    var path = string.Format(AddressableName.UILootItemPath, rw.Type);
                    m_SlotsReward[i].Clear();
                    ResourcesLoader.Instance.UnloadAsset<GameObject>(path);
                }

                var ePath = string.Format(AddressableName.UIHero, entity.Hero);
                ResourcesLoader.Instance.UnloadAsset<GameObject>(ePath);
                PoolManager.Instance.Despawn(actor);
            }

        };
        base.Close();
    }

    public void BuySaleOnClicked()
    {
        IAPManager.Instance.BuyProduct(entity.ProductId, OnCompletePurchased);
    }

    private async void Purchased()
    {
        saves.Get(entity.Hero).DeActiveHotSale();
        PanelManager.Instance.GetPanel<UIHotSaleHeroPanel>()?.UpdateUI();
        var uiRewardHero = await PanelManager.CreateAsync<UIChestRewardClaimHeroPanel>(AddressableName.UIChestRewardHeroPanel);
        uiRewardHero.Show(entity.Hero);
        uiRewardHero.onClosed = async () =>
        {
            var re = await PanelManager.ShowRewards(entity.Rewards);
            re.onClosed = Close;

            Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Product_Bought);

            DataManager.Save.User.UnlockHero(entity.Hero);
            DataManager.Save.User.SetPickHero(entity.Hero);

            Messenger.Broadcast(EventKey.UpdateHeroItemUI, entity.Hero);
            Messenger.Broadcast(EventKey.PickHero, entity.Hero);
        };
    }

    private void OnCompletePurchased(IAPManager.PurchaseState status, IAPPackage product)
    {
        if (status == IAPManager.PurchaseState.Success)
        {
            if (product.id == entity.ProductId)
            {
                Purchased();
            }
        }
    }
}
