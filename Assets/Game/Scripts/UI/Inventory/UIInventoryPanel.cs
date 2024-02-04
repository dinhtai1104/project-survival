using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using UI;
using UnityEngine;

public class UIInventoryPanel : UI.Panel
{
    [SerializeField] private UIInventoryAllEquipmentView inventoryView;
    [SerializeField] private UIEquipmentHeroView equipmentHeroView;
    [SerializeField] private UIInventoryHeroesView allHeroesView;
    [SerializeField] private UIInventoryHeroInforView heroInforView;
    private InventorySave inventorySave;
    private UserSave userSave;
    private PlayerData playerData;
    private void OnEnable()
    {
        Messenger.AddListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnEquipListener);
        Messenger.AddListener<EEquipment>(EventKey.UnEquipItem, OnUnEquipListener);
        Messenger.AddListener(EventKey.SortItemRarity, OnSortItemRarity);
    }


    private void OnDisable()
    {
        Messenger.RemoveListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnEquipListener);
        Messenger.RemoveListener<EEquipment>(EventKey.UnEquipItem, OnUnEquipListener);
        Messenger.RemoveListener(EventKey.SortItemRarity, OnSortItemRarity);
    }

    private void OnEquipListener(EEquipment type, EquipableItem item)
    {
        ShowInventory();
    }

    private void OnUnEquipListener(EEquipment type)
    {
        ShowInventory();
    }

    public override void PostInit()
    {
        playerData = GameSceneManager.Instance.PlayerData;
        inventorySave = DataManager.Save.Inventory;
        userSave = DataManager.Save.User;

    }

    public override void Show()
    {
        base.Show();
        ShowInventory();
        allHeroesView.Show(HeroOnClicked);
        HeroOnClicked(userSave.Hero);
    }

    public override void ShowByTransitions()
    {
        base.ShowByTransitions();
        HeroOnClicked(DataManager.Save.User.Hero);
    }

    private async void HeroOnClicked(EHero hero)
    {
        heroInforView.OnPickHero(hero);
        equipmentHeroView.OnPickHero(hero);

        if (userSave.IsUnlockHero(hero))
        {
            equipmentHeroView.ShowView();
            equipmentHeroView.ShowViewHeroUnlocked(hero);
            allHeroesView.Pick(hero);
            playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[hero].heroStat, EStatSource.sourceHero);
        }
        else
        {
            //equipmentHeroView.HideView();
            //equipmentHeroView.ShowViewHeroLocked(hero);
            HideByTransitions().Forget();
            var upgradeUI = await PanelManager.CreateAsync<UIHeroUpgradePanel>(AddressableName.UIHeroUpgradePanel);
            upgradeUI.Show();
            upgradeUI.SetHero(hero);
            upgradeUI.onClosed += () =>
            {
                ShowByTransitions();
            };
        }

        if (userSave.IsUnlockHero(hero) && hero != userSave.Hero)
        {
            userSave.SetPickHero(hero);
            Messenger.Broadcast(EventKey.PickHero, hero);
        }
    }


    [Button]
    public void ShowInventory()
    {
        inventoryView.Show();
        var equipmentHandler = GameSceneManager.Instance.PlayerData.EquipmentHandler;
        equipmentHeroView.Init(equipmentHandler);
    }
    private void SortInventoryByRarity()
    {
        inventorySave.SortByRarity();
        inventoryView.Show();
        var equipmentHandler = GameSceneManager.Instance.PlayerData.EquipmentHandler;
        equipmentHeroView.Init(equipmentHandler);
    }

    private void OnSortItemRarity()
    {
        SortInventoryByRarity();
    }
    public override void Close()
    {
        allHeroesView.Close();
        inventoryView.Clear();

        var playerData = GameSceneManager.Instance.PlayerData;
     //   playerData.Stats.RemoveModifiersFromSource("Hero");
        var userSave = DataManager.Save.User;
        var currentHero = userSave.Hero;
        var heroEntity = DataManager.Base.Hero.GetHero(currentHero);
        // Hero Add Stat Source
        // If we want to remove all stat of hero => removeAllStatFromSource("Hero");
       // heroEntity.SetBaseStat(playerData.Stats, "Hero");

        playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[heroEntity.TypeHero].heroStat, EStatSource.sourceHero);

        base.Close();
    }
}
