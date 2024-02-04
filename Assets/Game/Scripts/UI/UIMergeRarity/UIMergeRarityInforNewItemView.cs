using Assets.Game.Scripts.Utilities;
using I2.Loc;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMergeRarityInforNewItemView : MonoBehaviour
{
    [SerializeField] private Image rarityImge;
    [SerializeField] private TextMeshProUGUI nameItem;
    [SerializeField] private UIValueChanged levelMaxChange;
    [SerializeField] private UIValueChanged baseStatChange;
    [SerializeField] private TextMeshProUGUI hiddenAffixChange;

    private EquipableItem newItem;
    public void OnEnable()
    {
        Clear();
        Messenger.AddListener(EventKey.ChangeLanguage, ShowInfor);
    }

    public void OnDisable()
    {
        Messenger.RemoveListener(EventKey.ChangeLanguage, ShowInfor);
    }
    
    private void ShowInfor()
    {
        if (newItem == null) return;
        rarityImge.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, $"{newItem.EquipmentRarity}_Label");

        levelMaxChange.gameObject.SetActive(true);
        baseStatChange.gameObject.SetActive(true);

        nameItem.SetText(I2Localize.GetLocalize($"Equipment_Name/{newItem.Id}"));
        var entity = newItem.EquipmentEntity;

        // Set change rarity
        var oldRarity = newItem.EquipmentRarity - 1;
        var rarityOld = DataManager.Base.EquipmentRarity.GetLevel(oldRarity);
        var rarityNew = DataManager.Base.EquipmentRarity.GetLevel(newItem.EquipmentRarity);
        levelMaxChange.SetValue(I2Localize.GetLocalize("Common/LevelMax"), rarityOld.MaxLevel.TruncateValue(), rarityNew.MaxLevel.TruncateValue());

        // Set change base stat
        var baseStatKey = entity.StatKey;
        var baseStat = entity.StatBase;

        var oldBaseStat = baseStat + (newItem.BaseStatAffix.StatName == StatKey.Dmg ? rarityOld.DmgBase : rarityOld.HpBase).Value;
        var newBaseStat = baseStat + (newItem.BaseStatAffix.StatName == StatKey.Dmg ? rarityNew.DmgBase : rarityNew.HpBase).Value;
        baseStatChange.SetValue(I2Localize.GetLocalize($"Stat/{baseStatKey}"), oldBaseStat.TruncateValue(), newBaseStat.TruncateValue());
    
        // Set hidden affix
        if (newItem.TryGetLineAffix(rarityNew.Rarity, out string hiddenLine))
        {
            hiddenAffixChange.gameObject.SetActive(true);
            hiddenAffixChange.SetText(hiddenLine);
        }
    }
    public void Set(EquipableItem item)
    {
        Clear();
        this.newItem = item;
        ShowInfor();
    }
    public void Clear()
    {
        newItem = null;
        nameItem.SetText("");
        levelMaxChange.gameObject.SetActive(false);
        baseStatChange.gameObject.SetActive(false);
        hiddenAffixChange.gameObject.SetActive(false);
    }
}