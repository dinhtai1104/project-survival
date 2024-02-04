using System;
using System.Collections.Generic;

[System.Serializable]
public class EquipmentFactory
{
    private EquipmentTable _equipmentTable;
    private BlackSmithUpgradeTable _blackSmithTable;
    private EquipmentRarityTable _rarityTable;
    private PlayerData playerData;
    //private CurrencySave _currencySave;
    //private FragmentSave _fragmentSave;
    private ResourcesSave _resourceSave;

    public EquipmentFactory(PlayerData playerData)
    {
        _equipmentTable = DataManager.Base.Equipment;
        _blackSmithTable = DataManager.Base.BlackSmithUpgrade;
        this.playerData = playerData;
        _rarityTable = DataManager.Base.EquipmentRarity;

        _resourceSave = DataManager.Save.Resources;
    }

    public void EquipItem(EquipmentHandler handler)
    {
        var save = DataManager.Save.Inventory;
        int index = 0;
        foreach (var data in save.Saves)
        {
            if (data.IsEquiped)
            {
                handler.Equip(save.Find(data.IdSave));
            }
            index++;
        }
    }

    #region Modify Item

    public void ReverseEquipment(EquipableItem item)
    {
        ApplyLevelItem(item, 0);
    }

    public void ApplyLevelItem(EquipableItem item, int level)
    {
        ERarity rarity = item.EquipmentRarity;
        //Clear old enhace and rarity
        item.BaseEquipmentValue.ClearModifiers();
        item.BaseEquipmentValue.AddModifier(item.BaseStatAffix.StatName == StatKey.Dmg ? _rarityTable.Dictionary[rarity].DmgBase : _rarityTable.Dictionary[rarity].HpBase);
        item.BaseStatGrown.ClearModifiers();
        item.BaseStatGrown.AddModifier(item.BaseStatAffix.StatName == StatKey.Dmg ? _rarityTable.Dictionary[rarity].DmgGrown : _rarityTable.Dictionary[rarity].HpGrown);


        // apply new enhance and rarity
        var baseStat = item.BaseEquipmentValue.Value;
        baseStat += item.BaseStatGrown.Value * level;

        item.AddStat.ClearModifiers();
        item.AddStat.AddModifier(new StatModifier(EStatMod.Flat, item.BaseStatGrown.Value * item.ItemLevel));

        item.BaseStatAffix.ChangeModifier(baseStat);
        item.SetBaseStat(item.BaseStatAffix);
        item.ApplySaveData(item.EquipmentSave);
    }
    public void ApplyRarityItem(EquipableItem item, ERarity rarity)
    {
        var valueBaseStat = item.BaseStatAffix;
        item.EquipmentSave.SetRarity(rarity, true);
        // set new apply to new rarity
        item.BaseEquipmentValue.ClearModifiers();
        item.BaseEquipmentValue.AddModifier(item.BaseStatAffix.StatName == StatKey.Dmg ? _rarityTable.Dictionary[rarity].DmgBase : _rarityTable.Dictionary[rarity].HpBase);
        item.BaseStatGrown.ClearModifiers();
        item.BaseStatGrown.AddModifier(item.BaseStatAffix.StatName == StatKey.Dmg ? _rarityTable.Dictionary[rarity].DmgGrown : _rarityTable.Dictionary[rarity].HpGrown);


        // apply new enhance and rarity
        var baseStat = item.BaseEquipmentValue.Value;
        baseStat += item.BaseStatGrown.Value * item.ItemLevel;

        item.AddStat.ClearModifiers();
        item.AddStat.AddModifier(new StatModifier(EStatMod.Flat, item.BaseStatGrown.Value * item.ItemLevel));
        item.BaseStatAffix.ChangeModifier(baseStat);
        item.SetBaseStat(item.BaseStatAffix);
        item.ApplySaveData(item.EquipmentSave);
    }

    public bool EnhanceItem(EquipableItem item, out EEhanceResult result)
    {
        var eqEntity = _equipmentTable.GetEntity(item.Id);
        // Check level
        if (!CanEnhance(item))
        {
            result = EEhanceResult.MaxLevel;
            return false;
        }
        // Check Cost
        result = CheckEnhance(item);
        if (result != EEhanceResult.Success)
        {
            return false;
        }

        item.Enhance();
        ApplyLevelItem(item, item.ItemLevel);
        return true;
    }

    public EEhanceResult CheckEnhance(EquipmentSave item)
    {
        var entity = DataManager.Base.Equipment.Get(item.Id);
        var upgradeEntity = _blackSmithTable.GetResourceUpgradeEntity(entity.Type, item.Level + 1);
        if (upgradeEntity == null)
        {
            return EEhanceResult.MaxLevel;
        }
        if (!_resourceSave.HasResource(upgradeEntity.GoldCost))
        {
            return EEhanceResult.NotEnoughCurrency;
        }
        var fragment = upgradeEntity.MaterialCost.Clone();
        if (!_resourceSave.HasResource(fragment))
        {
            return EEhanceResult.NotEnoughFragment;
        }


        return EEhanceResult.Success;
    }
    public List<EEhanceResult> CheckEnhanceList(EquipableItem item)
    {
        var upgradeEntity = _blackSmithTable.GetResourceUpgradeEntity(item.EquipmentType, item.ItemLevel + 1);
        var res = new List<EEhanceResult>();
        if (upgradeEntity == null)
        {
            res.Add(EEhanceResult.MaxLevel);
        }
        if (!_resourceSave.HasResource(upgradeEntity.GoldCost))
        {
            res.Add(EEhanceResult.NotEnoughCurrency);
        }
        var fragment = upgradeEntity.MaterialCost.Clone();
        if (!_resourceSave.HasResource(fragment))
        {
            res.Add(EEhanceResult.NotEnoughFragment);
        }
        if (res.Count == 0)
        {
            res.Add(EEhanceResult.Success);
        }

        return res;
    }
    public EEhanceResult CheckEnhance(EquipableItem item)
    {
        var upgradeEntity = _blackSmithTable.GetResourceUpgradeEntity(item.EquipmentType, item.ItemLevel + 1);
        if (upgradeEntity == null)
        {
            return EEhanceResult.MaxLevel;
        }
        if (!_resourceSave.HasResource(upgradeEntity.GoldCost))
        {
            return EEhanceResult.NotEnoughCurrency;
        }
        var fragment = upgradeEntity.MaterialCost.Clone();
        if (!_resourceSave.HasResource(fragment))
        {
            return EEhanceResult.NotEnoughFragment;
        }


        return EEhanceResult.Success;
    }
    public EEhanceResult CheckEnhance(EquipmentData item)
    {
        var upgradeEntity = _blackSmithTable.GetResourceUpgradeEntity(item.Type, item.Level + 1);
        if (upgradeEntity == null)
        {
            return EEhanceResult.MaxLevel;
        }
        if (!_resourceSave.HasResource(upgradeEntity.GoldCost))
        {
            return EEhanceResult.NotEnoughCurrency;
        }
        var fragment = upgradeEntity.MaterialCost.Clone();
        if (!_resourceSave.HasResource(fragment))
        {
            return EEhanceResult.NotEnoughFragment;
        }


        return EEhanceResult.Success;
    }
    public bool CanEnhance(EquipableItem currentItem)
    {
        var levelCurrent = currentItem.ItemLevel;
        var maxLevelOfRarity = _rarityTable.Dictionary[currentItem.EquipmentRarity];
        if (levelCurrent >= maxLevelOfRarity.MaxLevel)
        {
            return false;
        }
        if (_blackSmithTable.Dictionary[currentItem.EquipmentType].Count <= currentItem.ItemLevel)
        {
            return false;
        }
        return true;
    }
    public bool CanEnhance(EquipmentData currentItem)
    {
        var levelCurrent = currentItem.Level;
        var maxLevelOfRarity = _rarityTable.Dictionary[currentItem.Rarity];
        if (levelCurrent >= maxLevelOfRarity.MaxLevel)
        {
            return false;
        }
        if (_blackSmithTable.Dictionary[currentItem.Type].Count <= currentItem.Level)
        {
            return false;
        }
        return true;
    }


    public bool UpRarityItem(EquipableItem item)
    {
        if (!CanUpRarityItem(item))
        {
            return false;
        }
        item.UpRarity();

        ApplyRarityItem(item, item.EquipmentRarity);
        return true;
    }
    private bool CanUpRarityItem(EquipableItem currentItem)
    {
        var currentRarity = currentItem.EquipmentRarity;
        if (currentRarity < ERarity.Ultimate)
        {
            return true;
        }
        return false;
    }

    public EquipableItem CreateEquipment(EquipmentData data)
    {
        var eq = DataManager.Base.Equipment.GetItem(data.IdString);
        eq.ApplySaveData(data.CreateSave());
        GameSceneManager.Instance.EquipmentFactory.ApplyLevelItem(eq, data.Level);
        GameSceneManager.Instance.EquipmentFactory.ApplyRarityItem(eq, data.Rarity);
        return eq;
    }
    #endregion
}