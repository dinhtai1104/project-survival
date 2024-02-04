using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InventorySave : BaseDatasave
{
    public delegate void OnChangeItem(EquipmentSave save);
    [NonSerialized] public OnChangeItem onChangeItemEvent;

    public int LastIdSave = 0;
    public List<EquipmentSave> Saves;
    private List<EquipableItem> allEquipableItem;
    public List<EquipableItem> AllEquipableItem => allEquipableItem;
    private EquipmentFactory equipmentFactory;

    public InventorySave()
    {
        Saves = new List<EquipmentSave>();
    }

    public InventorySave(string key) : base(key)
    {
        Saves = new List<EquipmentSave>();
        Saves.Add(new EquipmentSave { Id = "RifleGun", Level = 0, Rarity = ERarity.Common, IsEquiped = true });
    }

    public override void Fix()
    {
        for (int i = Saves.Count - 1; i >= 0; i--)
        {
            var save = Saves[i];
            if (!DataManager.Base.Equipment.HasEquipment(save.Id))
            {
                Saves.RemoveAt(i);
            }
        }
    }

    public void Add(EquipmentSave save)
    {
        LastIdSave++;
        save.IdSave = LastIdSave;
        Saves.Add(save);
        var item = save.CreateEquipableItem();
        equipmentFactory.ApplyLevelItem(item, save.Level);
        equipmentFactory.ApplyRarityItem(item, save.Rarity); 
        
        AllEquipableItem.Add(item);
        onChangeItemEvent?.Invoke(save);
        Save();
    }
    public void CreateAllEquipment(EquipmentFactory factory)
    {
        this.equipmentFactory = factory;
        Logger.Log("CreateAllEquipment");
        allEquipableItem = new List<EquipableItem>();
        foreach (var save in Saves)
        {
            var item = save.CreateEquipableItem();
            item.IsEquipped = save.IsEquiped;
            factory.ApplyLevelItem(item, save.Level);
            factory.ApplyRarityItem(item, save.Rarity);
            AllEquipableItem.Add(item);
        }
    }

    [Button]
    public EquipableItem AddEquipment(string Id, ERarity rarity, int Level)
    {
        LastIdSave++;
        var save = new EquipmentSave { Id = Id, Level = Level, Rarity = rarity, IdSave = LastIdSave };
        Saves.Add(save);
        var item = save.CreateEquipableItem();

        equipmentFactory.ApplyLevelItem(item, save.Level);
        equipmentFactory.ApplyRarityItem(item, save.Rarity);

        AllEquipableItem.Add(item);
        onChangeItemEvent?.Invoke(save);

        return item;
    }

    public EquipmentSave FindSave(int idSave)
    {
        return Saves.Find(t => t.IdSave == idSave);
    }

    public EquipableItem Find(int idSave)
    {
        Logger.Log(AllEquipableItem == null);
        return AllEquipableItem.Find(Pre);

        bool Pre(EquipableItem t)
        {
            Logger.Log("FIND " + idSave);
            Logger.Log(t.IdSave);
            Logger.Log(t.EquipmentSave == null);
            Logger.Log(t.EquipmentSave.IdSave);
            return t.EquipmentSave.IdSave == idSave;
        }

    }

    public void SortByRarity()
    {
        //var moduleSort = new EquipmentableItemSort();
        //Saves = Saves.OrderBy(eq => eq.Rarity).ThenBy(eq => eq.Level).Reverse().ToList();
        //allEquipableItem = allEquipableItem.OrderBy(eq => eq.EquipmentRarity).ThenBy(eq => eq.ItemLevel).Reverse().ToList();
        //Save();
    }

    public void RemoveItem(int idSave)
    {
        onChangeItemEvent?.Invoke(FindSave(idSave));
        allEquipableItem.RemoveAll(t => t.IdSave == idSave);
        Saves.RemoveAll(t=>t.IdSave == idSave);
        Save();
    }

    public void RemoveAllFromMergeRarity()
    {
        Saves.ForEach(e => { e.IsMaterialCrafting = e.IsSourceCrafting = false; });
    }
}

public class EquipmentableItemSort : IComparer<EquipableItem>, IComparer<EquipmentSave>
{
    public int Compare(EquipableItem itemA, EquipableItem itemB)
    {
        var rarityA = itemA.EquipmentRarity;
        var levelA = itemB.ItemLevel;

        var rarityB = itemB.EquipmentRarity;
        var levelB = itemB.ItemLevel;

        return SortRarity(rarityA, levelA, rarityB, levelB);
    }
    public int Compare(EquipmentSave itemA, EquipmentSave itemB)
    {
        var rarityA = itemA.Rarity;
        var levelA = itemB.Level;

        var rarityB = itemB.Rarity;
        var levelB = itemB.Level;

        return SortRarity(rarityA, levelA, rarityB, levelB);
    }

    public int SortRarity(ERarity rarityA, int levelA, ERarity rarityB, int levelB)
    {
        if (rarityA == rarityB && levelA == levelB) return 0;
        if (rarityA < rarityB) return -1;
        if (rarityA == rarityB)
        {
            if (levelA < levelB) return -1;
            return 1;
        }
        return 1;
    }
}
