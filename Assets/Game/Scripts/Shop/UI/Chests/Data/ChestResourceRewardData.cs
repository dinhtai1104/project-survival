using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class ChestResourceRewardData : ChestRewardBaseData
{
    public EResource ResourceType;
    public int Min;
    public int Max;

    public int Value => UnityEngine.Random.Range(Min, Max + 1);
    public ResourceData resourceData => new ResourceData { Resource = ResourceType, Value = Value };
    public override ILootData Data => resourceData;
    public ChestResourceRewardData(BGEntity e) : base(e)
    {
        Enum.TryParse(e.Get<string>("Id"), out ResourceType);
        Max = e.Get<int>("Max");
        Min = e.Get<int>("Min");

        if (ResourceType == EResource.EquipmentRdFragment || (ResourceType >= EResource.MainWpFragment && ResourceType <= EResource.BootFragment))
        {
            Type = ELootType.Fragment;
        }
        if (ResourceType == EResource.HeroRdFragment || (ResourceType >= EResource.NormalHero && ResourceType <= EResource.EvilHero))
        {
            Type = ELootType.HeroFragment;
        }
        if (ResourceType == EResource.HeroStoneRdFragment || (ResourceType >= EResource.BaseStone && ResourceType <= EResource.LightStone))
        {
            Type = ELootType.HeroStone;
        }
    }
}