using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInforStatPanel : UI.Panel
{
    [SerializeField] private RectTransform holder;
    private UIAffixText affixTextPrefab;
    [ShowInInspector]
    private PlayerData playerData;
    private List<UIAffixText> _affixes = new List<UIAffixText>();
    public override async void PostInit()
    {
        playerData = GameSceneManager.Instance.PlayerData;
        affixTextPrefab = (await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIAffixText)).GetComponent<UIAffixText>();

        RefreshStats();
    }
    private void OnEnable()
    {
        Messenger.AddListener<EquipableItem>(EventKey.EnhanceSuccess, OnEnhanceSuccess);
        Messenger.AddListener(EventKey.UpRaritySuccess, OnUpRaritySuccess);
        Messenger.AddListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnRefresh);
        Messenger.AddListener<EEquipment>(EventKey.UnEquipItem, OnRefresh);
    }

    private void OnRefresh(EEquipment type, EquipableItem item)
    {
        RefreshStats();
    }

    private void OnRefresh(EEquipment type)
    {
        RefreshStats();
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<EquipableItem>(EventKey.EnhanceSuccess, OnEnhanceSuccess);
        Messenger.RemoveListener(EventKey.UpRaritySuccess, OnUpRaritySuccess);
        Messenger.RemoveListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnRefresh);
        Messenger.RemoveListener<EEquipment>(EventKey.UnEquipItem, OnRefresh);
    }


    private void OnEnhanceSuccess(EquipableItem item)
    {
        RefreshStats();
    }

    private void OnUpRaritySuccess()
    {
        RefreshStats();
    }

    private void RefreshStats()
    {
        foreach (var affix in _affixes)
        {
            PoolManager.Instance.Despawn(affix.gameObject);
        }
        _affixes.Clear();

        ShowStat(StatKey.Dmg);
        ShowStat(StatKey.Hp);
        ShowStat(StatKey.SpeedMove);
        ShowStat(StatKey.CritRate);
        ShowStat(StatKey.CritDmg);
        ShowStat(StatKey.FireRate);
    }

    private void ShowStat(StatKey statKey)
    {
        var affix = PoolManager.Instance.Spawn(affixTextPrefab, holder);
        var statAffix = playerData.Stats.GetStat(statKey);
        var localizeKey = $"Affix/Base_{statKey}_Flat";
        var description = I2Localize.GetLocalize(localizeKey, statAffix.Value);
        affix.SetDescription(description);
        _affixes.Add(affix);
    }
}
