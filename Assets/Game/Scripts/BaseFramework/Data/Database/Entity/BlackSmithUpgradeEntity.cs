using System;

[System.Serializable]
public class BlackSmithUpgradeEntity
{
    public EEquipment EquipmentType;
    public EResource FragmentType;
    public int Level;

    public ResourceData GoldCost;
    public ResourceData MaterialCost;

    public BlackSmithUpgradeEntity(DB_BlackSmithUpgrade e)
    {
        Enum.TryParse(e.Get<string>("EquipmentType"), out EquipmentType);
        Enum.TryParse(e.Get<string>("ResourceType"), out FragmentType);
        Level = e.Get<int>("EnhanceLevel");
        var gold = e.Get<int>("EnhanceGold");

        GoldCost = new ResourceData { Resource = EResource.Gold, Value = gold };

        var ResourceCost = int.Parse(e.Get<string>("ResourceCost"));
        MaterialCost = new ResourceData { Resource = FragmentType, Value = ResourceCost };
    }
}