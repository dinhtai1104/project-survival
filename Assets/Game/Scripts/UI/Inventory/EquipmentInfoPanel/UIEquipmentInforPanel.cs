using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using FullSerializer.Internal;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentInforPanel : UI.Panel
{
    [SerializeField] private Image rarityImage;
    [SerializeField] private TextMeshProUGUI equipmentNameTxt;
    [SerializeField] private TextMeshProUGUI equipmentRarityTxt;
    [SerializeField] private TextMeshProUGUI equipmentLevelTxt;
    [SerializeField] private TextMeshProUGUI equipmentDescriptionTxt;
    [SerializeField] private UIInventorySlot equipmentSlot;
    [SerializeField] private UIStatItem baseStatItemInfo;
    [SerializeField] private ParticleSystem levelUpEffect;

    private EquipableItem currentItem;
    private EquipmentFactory _equipmentFactory;
    private EquipmentEntity equipmentEntity;

    [SerializeField] private UIGroupEquipmentStatAffixes groupStatAffixes;
    [SerializeField] private GameObject equipButton, unEquipButton, cannotUnEquip, buttonReverseEquipment, buttonEnhance, buttonMaxLevel;

    [SerializeField] private GameObject costPanel;
    [SerializeField] private Image currencyCostImg;
    [SerializeField] private TextMeshProUGUI currencyCostText;

    [SerializeField] private Image fragmentCostImg;
    [SerializeField] private TextMeshProUGUI fragmentCostText;

    [SerializeField] private UITweenRunner enhaceEffect;
    [SerializeField] private GameObject bottomButtons;

    private BlackSmithUpgradeTable BlackSmithUpgrade;

    private PlayerData playerData;
    private ResourcesSave resourcesSave;

    private ResourceData fragmentData;
    private ResourceData currencyData;

    private void OnEnable()
    {
        Messenger.AddListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverseEquipment);
        Messenger.AddListener<EResource>(EventKey.UpdateResource, OnUpdateResource);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverseEquipment);
        Messenger.RemoveListener<EResource>(EventKey.UpdateResource, OnUpdateResource);
    }

    private void OnUpdateResource(EResource res)
    {
        SetCostPanel();
    }

    private void OnReverseEquipment(EquipmentSave equipment)
    {
        var eq = currentItem;
        if (eq == null) return;
        var save = eq.EquipmentSave;
        if (save.Id == equipment.Id && save.IdSave == equipment.IdSave)
        {
            Show(currentItem);
        }
    }


    public override void PostInit()
    {
        BlackSmithUpgrade = DataManager.Base.BlackSmithUpgrade;
        playerData = GameSceneManager.Instance.PlayerData;
        _equipmentFactory = GameSceneManager.Instance.EquipmentFactory;
        resourcesSave = DataManager.Save.Resources;
    }
    public void Show(EquipableItem item, bool isTooltip = false)
    {
        base.Show();
        currentItem = item;
        equipmentEntity = item.EquipmentEntity;

        SetBaseInfo(item);
        if (isTooltip)
        {
            bottomButtons.SetActive(false);
            baseStatItemInfo.SetAddStat(0);
        }
        else
        {
            bottomButtons.SetActive(true);
        }
    }

    public void Show(EquipmentData data)
    {
        base.Show();
        currentItem = DataManager.Base.Equipment.GetItem(data.IdString);
        equipmentEntity = currentItem.EquipmentEntity;

        SetBaseInfo(currentItem);
    }

    private async void SetBaseInfo(EquipableItem item)
    {
        equipmentSlot.Clear();

        // Get icon equipment
        var generalItemPrefab = await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIGeneralEquipmentItem);
        var general = PoolManager.Instance.Spawn(generalItemPrefab).GetComponent<UIGeneralEquipmentIcon>();
        general.Init(item);
        equipmentSlot.SetItem(general);

        groupStatAffixes.Show(currentItem);
        RefreshUI(item);
    }

    private void RefreshUI(EquipableItem item)
    {
        rarityImage.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, $"{item.EquipmentRarity}_Label");
        equipmentNameTxt.text = I2Localize.GetLocalize($"Equipment_Name/{item.Id}");
        equipmentRarityTxt.text = I2Localize.GetLocalize($"Common/{item.EquipmentRarity}");
        equipmentLevelTxt.text = $"Lv. {item.ItemLevel}/{item.EquipmentRarity.GetMaxLevel()}";
        equipmentDescriptionTxt.text = I2Localize.GetLocalize($"Equipment_Description/{item.Id}");
        baseStatItemInfo.SetData(item.BaseStatAffix.StatName, item.BaseStatAffix.Value);

        equipButton.SetActive(!item.IsEquipped);
        unEquipButton.SetActive(item.IsEquipped);

        if (item.IsEquipped)
        {
            if (item.EquipmentType == EEquipment.MainWeapon)
            {
                unEquipButton.SetActive(false);
                cannotUnEquip.SetActive(true);
            }
        }

        buttonReverseEquipment.SetActive(item.ItemLevel > 0);
        buttonMaxLevel.SetActive(false);
        buttonEnhance.SetActive(true);

        SetCostPanel();
    }

    private void SetCostPanel()
    {
        costPanel.SetActive(true);
        var level = currentItem.ItemLevel;
        var upgradeEntity = BlackSmithUpgrade.GetResourceUpgradeEntity(currentItem.EquipmentType, level + 1);
        baseStatItemInfo.SetAddStat(0);

        if (upgradeEntity == null || !_equipmentFactory.CanEnhance(currentItem))
        {
            buttonMaxLevel.SetActive(true);
            buttonEnhance.SetActive(false);
            costPanel.SetActive(false);
            return;
        }

        currencyData = upgradeEntity.GoldCost;
        var currencyType = upgradeEntity.GoldCost.Resource;
        currencyCostImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{currencyType}");
        //currencyCostText.text = $"{resourcesSave.GetResource(currencyType).TruncateValue()}/{upgradeEntity.GoldCost.Value.TruncateValue()}";
        currencyCostText.text = $"{CSharpExtension.FormatRequire(resourcesSave.GetResource(currencyType), upgradeEntity.GoldCost.Value)}";

        var fragment = (upgradeEntity.MaterialCost as ResourceData).Clone();
        var fragmentValue = fragment.Value;
        var fragmentType = fragment.Resource;

        fragmentCostImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{fragmentType}");
        //fragmentCostText.text = $"{resourcesSave.GetResource(fragmentType).TruncateValue()}/{fragment.Value.TruncateValue()}";
        fragmentCostText.text = $"{CSharpExtension.FormatRequire(resourcesSave.GetResource(fragmentType), fragment.Value)}";

        fragmentData = fragment;

        if (resourcesSave.HasResource(fragmentData) && resourcesSave.HasResource(currencyData))
        {
            baseStatItemInfo.SetAddStat(currentItem.BaseStatGrown.Value);
        }
    }

    public async void ReverseItem()
    {
        var ui = await UI.PanelManager.CreateAsync<UIEquipmentReversePanel>(AddressableName.UIEquipmentRevesePanel);
        ui.Show(currentItem);
    }

    public async void EnhanceItem()
    {
        bool rs = _equipmentFactory.EnhanceItem(currentItem, out EEhanceResult result);
        if (rs)
        {
            enhaceEffect.Stop();
            enhaceEffect.Show().Forget();
            // Cost
            resourcesSave.DecreaseResources(currencyData, fragmentData);

            RefreshUI(currentItem);
            Messenger.Broadcast(EventKey.EnhanceSuccess, currentItem);

            Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.UpgradeEquipment);
            int maxLevel = DataManager.Save.Inventory.Saves.Max(t => t.Level);
            Architecture.Get<AchievementService>().SetProgress(EAchievement.UpgradeEquipmentLevel, maxLevel);

            // Track
            FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
                .AddStringParam("item_category", currencyData.GetAllData()[0].Type.ToString())
                .AddStringParam("item_id", currencyData.Resource.ToString())
                .AddStringParam("source", "level_up")
                .AddIntParam("source_id", currentItem.ItemLevel)
                .AddDoubleParam("value", currencyData.Value)
                .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(currencyData.Resource))
                .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(currencyData.Resource))
                .Track();

            FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
                .AddStringParam("item_category", fragmentData.GetAllData()[0].Type.ToString())
                .AddStringParam("item_id", fragmentData.Resource.ToString())
                .AddStringParam("source", "level_up")
                .AddIntParam("source_id", currentItem.ItemLevel)
                .AddDoubleParam("value", fragmentData.Value)
                .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(fragmentData.Resource))
                .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(fragmentData.Resource))
                .Track();


            var results = _equipmentFactory.CheckEnhanceList(currentItem);
            foreach (var res in results)
            {
                switch (res)
                {
                    case EEhanceResult.MaxLevel:
                        break;
                    case EEhanceResult.NotEnoughCurrency:
                        MenuGameScene.Instance.EnQueue(EFlashSale.Gold);
                        break;
                    case EEhanceResult.NotEnoughFragment:
                        MenuGameScene.Instance.EnQueue(EFlashSale.Scroll);
                        break;
                }
            }
        }
        else
        {
            
            UINoticePanel notice = null;

            switch (result)
            {
                case EEhanceResult.MaxLevel:
                    UI.PanelManager.ShowNotice("Max Level").Forget();
                    break;
                case EEhanceResult.NotEnoughCurrency:
                    notice = await PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, currencyData.Resource.GetLocalize()));
                    break;
                case EEhanceResult.NotEnoughFragment:
                    notice = await PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, fragmentData.Resource.GetLocalize()));
                    break;
            }
            if (notice != null)
            {
                var results = _equipmentFactory.CheckEnhanceList(currentItem);
                notice.onClosed += () =>
                {
                    foreach (var res in results)
                    {
                        switch (res)
                        {
                            case EEhanceResult.MaxLevel:
                                break;
                            case EEhanceResult.NotEnoughCurrency:
                                MenuGameScene.Instance.EnQueue(EFlashSale.Gold);
                                break;
                            case EEhanceResult.NotEnoughFragment:
                                MenuGameScene.Instance.EnQueue(EFlashSale.Scroll);
                                break;
                        }
                    }
                };
            }
        }
    }

    private async void ShowFlashSale(List<EEhanceResult> results)
    {
        UIFlashSalePanel ui;
        if (results.Count == 0) return;
        var result = results.GroupBy(t=>t).Select(t=>t.First()).ToList();
        foreach (var res in result)
        {
            switch (res)
            {
                case EEhanceResult.MaxLevel:
                    break;
                case EEhanceResult.NotEnoughCurrency:
                    ui = await PanelManager.CreateAsync<UIFlashSalePanel>(AddressableName.UIFlashSalePanel.AddParams(EFlashSale.Gold));
                    ui.Show(EFlashSale.Gold);

                    break;
                case EEhanceResult.NotEnoughFragment:
                    ui = await PanelManager.CreateAsync<UIFlashSalePanel>(AddressableName.UIFlashSalePanel.AddParams(EFlashSale.Scroll));
                    ui.Show(EFlashSale.Gold);
                    break;
            }
            break;
        }
    }


    public void UpRarityItem()
    {
        bool rs = _equipmentFactory.UpRarityItem(currentItem);
        if (rs)
        {
            SetBaseInfo(currentItem);
            Messenger.Broadcast(EventKey.UpRaritySuccess);
        }
    }
    public void EquipItemOnClicked()
    {
        // Track
        FirebaseAnalysticController.Tracker.NewEvent("hero_equipment")
            .AddStringParam("action", "equip")
            .AddStringParam("hero_id", DataManager.Save.User.Hero.ToString())
            .AddStringParam("equipment_id", currentItem.Id)
            .Track();


        playerData.EquipmentHandler.Equip(currentItem);
        equipButton.SetActive(!currentItem.IsEquipped);
        unEquipButton.SetActive(currentItem.IsEquipped);
        Close();
    }
    public void UnEquipItemOnClicked()
    {
        if (currentItem.IsEquipped)
        {
            if (currentItem.EquipmentType == EEquipment.MainWeapon)
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Cannot UnEquip MainWeapon")).Forget();
                return;
            }
        }

        // Track
        FirebaseAnalysticController.Tracker.NewEvent("hero_equipment")
            .AddStringParam("action", "unequip")
            .AddStringParam("hero_id", DataManager.Save.User.Hero.ToString())
            .AddStringParam("equipment_id", currentItem.Id)
            .Track();

        playerData.EquipmentHandler.Unequip(currentItem.EquipmentType);
        equipButton.SetActive(!currentItem.IsEquipped);
        unEquipButton.SetActive(currentItem.IsEquipped);
        Close();
    }

    public override void Close()
    {
        groupStatAffixes.Clear();
        equipmentSlot.Clear();
        base.Close();
    }

    public void CheatFragment()
    {
#if DEVELOPMENT
        var fragType = fragmentData.Resource;

        resourcesSave.IncreaseResource(fragType, 100);
#endif
    }
}