using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mosframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayGiftPanel : UI.Panel
{
    [SerializeField] private DynamicHScrollView scrollView;
    private PlayGiftSaves save;
    private PlayGiftTable table;
    [SerializeField] private GameObject x3PurchaseButton;
    [SerializeField] private GameObject loadoutButton;
    [SerializeField] private Image maskUI;
    [SerializeField] private Transform holder;

    private void OnEnable()
    {
        Messenger.AddListener(EventKey.PurchaseX3Gift, OnUpdate);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener(EventKey.PurchaseX3Gift, OnUpdate);
    }

    private void OnUpdate()
    {
        x3PurchaseButton.SetActive(!DataManager.Save.PlayGift.Purchased);
    }

    public override void PostInit()
    {
        x3PurchaseButton.SetActive(!DataManager.Save.PlayGift.Purchased);
    }

    public override async void Show()
    {
        save = DataManager.Save.PlayGift;
        table = DataManager.Base.PlayGift;

        base.Show();
        scrollView.totalItemCount = table.Dictionary.Count;
        Timing.RunCoroutine(_Update(), gameObject);
        
    }

    private IEnumerator<float> _Update()
    {
        scrollView.refresh();
        yield return Timing.WaitForSeconds(0.3f);
        scrollView.refresh();

        if (save.Saves.Count > 0)
        {
            scrollView.scrollByItemIndex(save.Saves.Max() - 1);
        }
        scrollView.refresh();
    }

    private Sprite iconCurrent;
    public async void ClaimReward(int INDEX, Sprite icon = null)
    {
        if (save.IsClaimedPlay(INDEX)) return;
        save.Claim(INDEX);
        this.iconCurrent = icon;
        scrollView.refresh();
        var entity = table.Get(INDEX);
        var reward = await PanelManager.ShowRewards(entity.Reward);
        reward.onClosed += OnCloseReward;


        string resource = "";
        double remaining = 0;
        double earn = 0;
        switch (entity.Reward.Type)
        {
            case ELootType.Resource:
                resource = (entity.Reward.Data as ResourceData).Resource.ToString();
                remaining = DataManager.Save.Resources.GetResource((entity.Reward.Data as ResourceData).Resource);
                earn = FirebaseAnalysticController.Instance.GetTrackingResourceEarn((entity.Reward.Data as ResourceData).Resource);
                break;
            case ELootType.Equipment:
                resource = (entity.Reward.Data as EquipmentData).IdString.ToString();
                break;
            case ELootType.Hero:
                resource = (entity.Reward.Data as HeroData).HeroType.ToString();
                break;
            case ELootType.Exp:
                resource = "Exp";
                break;
        }
        // Track
        FirebaseAnalysticController.Tracker.NewEvent("earn_resource")
            .AddStringParam("item_category", entity.Reward.Type.ToString())
            .AddStringParam("item_id", resource)
            .AddStringParam("source", "play_gift")
            .AddIntParam("source_id", INDEX)
            .AddDoubleParam("value", entity.Reward.Data.ValueLoot)
            .AddDoubleParam("remaining_value", remaining)
            .AddDoubleParam("total_earned_value", earn)
            .Track();
    }

    private async void OnCloseReward()
    {
        var reward = PanelManager.Instance.GetPanel<UIRewardPanel>();
        //this.Close();
        maskUI.DOFade(0.75f, 0.2f);
        loadoutButton.SetActive(true);
        maskUI.gameObject.SetActive(true);

        var list = new List<UniTask>();
        foreach (var item in reward.InventorySlot)
        {
            var uiicon = item.Item.Sprite;
            var task = UIHelper.FloatIcon(uiicon, (transform as RectTransform).position 
                + (Vector3)UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0.5f, 1.5f), 
                loadoutButton.transform as RectTransform, parent: holder);

            list.Add(task);
        }
        await UniTask.WhenAll(list);
        maskUI.DOFade(0f, 0.2f);

        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        maskUI.gameObject.SetActive(false);
        loadoutButton.SetActive(false);
    }

    public void Refresh()
    {
        scrollView.refresh();
    }

    public void ClaimX3OnClicked()
    {
        PanelManager.CreateAsync<UIPlayGiftX3Panel>(AddressableName.UIPlayGiftX3Panel).ContinueWith(panel =>
        {
            panel.Show();
        }).Forget();
    }
}
