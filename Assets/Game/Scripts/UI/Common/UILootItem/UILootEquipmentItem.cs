public class UILootEquipmentItem : UILootItemBase
{
    public override void SetData(ILootData lootData)
    {
        var data = lootData as EquipmentData;

        SetAmount((int)data.ValueLoot);
    }
}
