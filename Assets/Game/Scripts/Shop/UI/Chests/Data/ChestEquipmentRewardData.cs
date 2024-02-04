
using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class ChestEquipmentRewardData : ChestRewardBaseData
{
    public string IdEquipment;
    public ERarity Rarity;

    public EquipmentData equipmentData;
    public override ILootData Data => equipmentData;

    public ChestEquipmentRewardData(BGEntity e) : base(e)
    {
        IdEquipment = e.Get<string>("Id");
        Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
        equipmentData = new EquipmentData(IdEquipment, Rarity);
    }
}