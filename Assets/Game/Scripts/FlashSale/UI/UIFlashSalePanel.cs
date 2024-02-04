using Assets.Game.Scripts.Utilities;
using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIFlashSalePanel : UI.Panel
{
    [SerializeField] protected UIInventorySlot uiRewardSlotPrefab;
    [SerializeField] protected TextMeshProUGUI titleTxt;
    [SerializeField] protected TextMeshProUGUI descriptionTxt;
    [SerializeField] protected TextMeshProUGUI timeLeftTxt;
    [SerializeField] protected Text priceTxt;
    [SerializeField] protected Text salePriceTxt;
    [SerializeField] protected Image iconImg;

    [SerializeField] protected UILootCollectionView rewardView;
    [SerializeField] protected Button claimButton;
    [SerializeField] protected TextMeshProUGUI m_XValueTxt;

    protected EFlashSale saleType;
    protected FlashSaleSave save;
    protected FlashSaleEntity entity;
    
    public override void PostInit()
    {
    }

    protected virtual void OnEnable()
    {
        claimButton.onClick.AddListener(ClaimFlashSale);
    }
    protected virtual void OnDisable()
    {
        claimButton.onClick.RemoveListener(ClaimFlashSale);
    }

    [Button]
    public virtual void Show(EFlashSale sale)
    {
        this.saleType = sale;
        base.Show();
        entity = DataManager.Base.FlashSale.Get(sale);
        save = DataManager.Save.FlashSale.GetSave(sale);

        m_XValueTxt.text = $"{entity.XValue}x";
        if (entity.XValue == 1)
        {
            m_XValueTxt.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            m_XValueTxt.transform.parent.gameObject.SetActive(true);
        }

        save.Active();
        onClosed += OnClosed;

        Timing.KillCoroutines(gameObject);
        UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        titleTxt.text = I2Localize.GetLocalize($"FlashSale/Title_{saleType}");
        descriptionTxt.text = I2Localize.GetLocalize($"FlashSale/Description_{saleType}");
        iconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.FlashSale, $"Img_{saleType}");
        iconImg.SetNativeSize();

        var price = IAPManager.Instance.GetPrice(entity.ProductId);
        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(entity.ProductId);

        priceTxt.text = $"{(price * 100 / (100 - entity.SaleOff)).PriceShow()} {isoCode}";
        salePriceTxt.text = $"{price.PriceShow()} {isoCode}";

        SetRewards(entity.Rewards);
        Timing.RunCoroutine(_Ticks());
    }

    private IEnumerator<float> _Ticks()
    {
        while (true)
        {
            var left = save.TimeEnd - UnbiasedTime.UtcNow;
            timeLeftTxt.text = I2Localize.GetLocalize("Common/Title_EndIn", left.ConvertTimeToString());
            if (left.TotalSeconds <= 0) break;

            yield return Timing.WaitForSeconds(1f);
        }
        Close();
    }

    protected virtual void ClaimFlashSale()
    {
        IAPManager.Instance.BuyProduct(entity.ProductId, OnPurchaseResult);
        
    }

    private void OnPurchaseResult(IAPManager.PurchaseState rs, IAPPackage package)
    {
        if (rs == IAPManager.PurchaseState.Success)
        {
            if (package.id == entity.ProductId)
            {
                save.Claim();
                Close();
                PanelManager.ShowRewards(entity.Rewards).Forget();
                Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Product_Bought);
            }
        }
    }

    private void OnClosed()
    {
        if (save.IsClaimed) return;
        if (save.IsShow())
        {
            return;
        }
        save.IsShowed = true;
        DataManager.Save.FlashSale.Save();

        return;

        // Float Ui to pos


        //var uifloat = ResourcesLoader.Instance.Get<UIFloatIcon>(AddressableName.UIFloatIcon, PanelManager.Instance.transform);
        //var menu = PanelManager.Instance.GetPanel<UIMainMenuPanel>();
        //PoolManager.Instance.Clear(uiRewardSlotPrefab.gameObject);
        //var target = menu.FindFSButton(saleType);
        //target.GetComponent<CanvasGroup>().alpha = 0;
        //target.SetActive(true);
        //var featureButton = ResourcesLoader.Instance.GetSprite("FeatureButton", "FS_" + saleType.ToString());
        //uifloat.Set(featureButton, (rewardView.transform as RectTransform).position, target.GetComponent<RectTransform>(), 0.6f, (t) =>
        //{
        //    ResourcesLoader.Instance.GetAsync<ParticleSystem>("VFX_UIFloat_Icon", t.transform).ContinueWith(effect =>
        //    {
        //        effect.transform.localPosition = Vector3.zero;
        //        t.GetComponent<CanvasGroup>().alpha = 1;
        //        effect.Play();
        //    }).Forget();

        //    target.transform.DOScale(Vector3.one * 0.7f, 0.1f).SetEase(Ease.InSine).OnComplete(() =>
        //    {
        //        target.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnKill(() =>
        //        {
        //            target.transform.localScale = Vector3.one;
        //        });
        //    }).OnKill(() =>
        //    {
        //        target.transform.localScale = Vector3.one;
        //    });
        //});
        //uifloat.Run().Forget();
    }

    protected void SetRewards(List<LootParams> rewards)
    {
        rewardView.Show(new LootCollectionData(rewards), this.uiRewardSlotPrefab).Forget();
    }
}
