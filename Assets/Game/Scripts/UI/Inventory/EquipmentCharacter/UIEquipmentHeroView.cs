using Cysharp.Threading.Tasks;
using I2.Loc;
using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class UIEquipmentHeroView : MonoBehaviour
{
    [SerializeField] private UIInventoryEquipmentSlot[] _allSlot;
    [SerializeField] private RectTransform uiCharacterHolder;
    [SerializeField] private RectTransform uiDroneHolder;
    [SerializeField] private List<UITweenBase> tweens;
    [SerializeField] private GameObject buttonUpgradeHero;
    [SerializeField] private GameObject buttonUnlockHeroHero;
    [SerializeField] private UIIconText resourceToUnlockHero;
    [SerializeField] private HeroUpgradeLevelNotifyCondition heroUpgradeLevelCondition;
    [SerializeField] private HeroUpgradeStarNotifyCondition heroUpgradeStarCondition;
    private ResourceData costUnlockHero;

    private Dictionary<EEquipment, UIInventoryEquipmentSlot> _slotEquipmentsUI;
    private EHero hero;
    private UIPlayerActor uiActor;
    private UIActor uiDrone;

    public void HideView()
    {
        foreach (var tween in tweens)
        {
            tween.Hide();
        }
    }
    public void ShowView()
    {
        foreach (var tween in tweens)
        {
            tween.Show();
        }
    }

    public void ShowViewHeroUnlocked(EHero hero)
    {
        this.hero = hero;
        buttonUnlockHeroHero.SetActive(false);
        buttonUpgradeHero.SetActive(heroUpgradeStarCondition.Validate(hero) || heroUpgradeLevelCondition.Validate(hero) || DataManager.Save.User.IsUnlockHero(hero));
    }
    public void ShowViewHeroLocked(EHero hero)
    {
        this.hero = hero;
        buttonUnlockHeroHero.SetActive(true);
        buttonUpgradeHero.SetActive(false);
        var data = DataManager.Base.Hero.Get(hero);
        costUnlockHero = new ResourceData(EResource.NormalHero + (int)hero, data.NumberFragmentUnlock);

        string fragmentAmount = "{0}/<color={1}>{2}</color>";
        var require = data.NumberFragmentUnlock;
        var origin = DataManager.Save.Resources.GetResource(EResource.NormalHero + (int)hero);
        var color = require > origin ? "red" : "green";

        fragmentAmount = $"{CSharpExtension.FormatRequire(origin, require)}";

        resourceToUnlockHero.Set(costUnlockHero);
        resourceToUnlockHero.SetAmountFormat(fragmentAmount);
    }

    private void OnReverseEquipment(EquipmentSave equipment)
    {
        foreach (var slot in _slotEquipmentsUI)
        {
            var eq = slot.Value.SlotItem;
            if (eq == null) continue;
            var save = eq.Item.EquipmentSave;
            if (save.Id == equipment.Id && save.IdSave == equipment.IdSave)
            {
                eq.OnUpdate();
            }
            slot.Value.GetComponent<UIInventorySlot>().SetNoti(CheckNoti(eq.Item));
        }
    }

    private bool CheckNoti(EquipableItem item)
    {
        var res = GameSceneManager.Instance.EquipmentFactory.CheckEnhance(item);

        switch (res)
        {
            case EEhanceResult.Success:
                return true;
            case EEhanceResult.MaxLevel:
                return false;
            case EEhanceResult.NotEnoughCurrency:
                return false;
            case EEhanceResult.NotEnoughFragment:
                return false;
        }
        return false;
    }

    private void OnEnable()
    {
        Messenger.AddListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnEquipListener);
        Messenger.AddListener<EEquipment>(EventKey.UnEquipItem, OnUnEquipListener);
        Messenger.AddListener<EHero>(EventKey.PickHero, OnPickHero);
        Messenger.AddListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverseEquipment);
        Messenger.AddListener<EquipableItem>(EventKey.EnhanceSuccess, OnEnhanceSuccess);
    }

    private void OnUnEquipListener(EEquipment type)
    {
        _slotEquipmentsUI[type].UnEquip();

        if (type == EEquipment.Drone)
        {
            if (uiDrone)
            {
                PoolManager.Instance.Despawn(uiDrone.gameObject);
            }
        }
    }

    private void OnEquipListener(EEquipment type, EquipableItem item)
    {
        _slotEquipmentsUI[type].Equip(item);
        if (type == EEquipment.Drone)
        {
            if (uiDrone)
            {
                PoolManager.Instance.Despawn(uiDrone.gameObject);
            }
            var name = item.EquipmentEntity.Id;
            uiDrone = ResourcesLoader.Instance.Get<UIActor>("DroneIcon/" + name + ".prefab", uiDroneHolder);
        }
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnEquipListener);
        Messenger.RemoveListener<EEquipment>(EventKey.UnEquipItem, OnUnEquipListener);
        Messenger.RemoveListener<EHero>(EventKey.PickHero, OnPickHero);
        Messenger.RemoveListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverseEquipment);
        Messenger.RemoveListener<EquipableItem>(EventKey.EnhanceSuccess, OnEnhanceSuccess);
    }

    private void OnEnhanceSuccess(EquipableItem item)
    {
        foreach (var slot in _slotEquipmentsUI)
        {
            var eq = slot.Value.SlotItem;
            if (eq == null) continue;
            slot.Value.GetComponent<UIInventorySlot>().SetNoti(CheckNoti(eq.Item));
        }
    }

    private EHero lastHeroPick = EHero.None;
    public void OnPickHero(EHero eHero)
    {
        if (eHero != lastHeroPick)
        {
            // Set New HeroSkin
            if (uiActor)
            {
                PoolManager.Instance.Despawn(uiActor.gameObject);
            }
            var ePath = string.Format(AddressableName.UIHero, eHero);
            uiActor = ResourcesLoader.Instance.Get<UIActor>(ePath, uiCharacterHolder) as UIPlayerActor;
            uiActor.PlayVFXAppear();
        }
        lastHeroPick = eHero;
    }

    public void Init(EquipmentHandler handler)
    {
        _slotEquipmentsUI = new Dictionary<EEquipment, UIInventoryEquipmentSlot>();
        foreach (var slot in _allSlot)
        {
            var type = slot.Type;
            _slotEquipmentsUI.Add(type, slot);

            if (handler.HasEquipmentType(type))
            {
                slot.Equip(handler.GetEquipment(type));
                if (type == EEquipment.Drone) 
                {
                    if (uiDrone)
                    {
                        PoolManager.Instance.Despawn(uiDrone.gameObject);
                    }
                    var name = handler.GetEquipment(type).EquipmentEntity.Id;
                    uiDrone = ResourcesLoader.Instance.Get<UIActor>("DroneIcon/" + name + ".prefab", uiDroneHolder);
                }
            }
        }
        OnPickHero(DataManager.Save.User.Hero);
    }

    public async void UnlockHeroOnClickedAsync()
    {
        if (DataManager.Save.Resources.HasResource(costUnlockHero))
        {
            DataManager.Save.Resources.DecreaseResource(costUnlockHero);
            DataManager.Save.User.UnlockHero(hero);
            DataManager.Save.User.SetPickHero(hero);

            Messenger.Broadcast(EventKey.UpdateHeroItemUI, hero);
            Messenger.Broadcast(EventKey.PickHero, hero);
            OnPickHero(DataManager.Save.User.Hero);

            var ui = await PanelManager.CreateAsync<UIChestRewardClaimHeroPanel>(AddressableName.UIChestRewardHeroPanel);
            ui.Show(hero);

            ShowView();
            ShowViewHeroUnlocked(hero);
        }
        else
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, costUnlockHero.Resource.GetLocalize())).Forget();
        }
    }
}