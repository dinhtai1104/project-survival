using System;
using System.Collections.Generic;

[System.Serializable]
public class AffixEquipmentEntity
{
    public string Id;
    public ERarity Rarity;
    public List<string> Equipment;
    public AttributeStatModifier Attribute;
    public string TypeAffix;

    public string DescriptionKey;

    public AffixEquipmentEntity(BansheeGz.BGDatabase.BGEntity e)
    {
        Id = e.Get<string>("Id");
        Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
        Equipment = e.Get<List<string>>("Equipment");
        Attribute = new AttributeStatModifier(e.Get<string>("Params"));
        TypeAffix = e.Get<string>("TypeAffix");

        DescriptionKey = $"{TypeAffix}_{Attribute.StatKey}_{Attribute.Modifier.Type}";
    }
}