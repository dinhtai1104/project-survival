using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class LootParams
{
    public ELootType Type;
    public List<string> Params;
    public string DataRaw;
    [ShowInInspector]
    public ILootData data;

    public virtual ILootData Data { get => data; set => data = value; }

    public LootParams() { }

    public LootParams(ELootType type, ILootData data)
    {
        this.Type = type;
        this.Data = data;
    }

    public LootParams(string data)
    {
        if (string.IsNullOrEmpty(data)) return;
        DataRaw = data;
        Params = new List<string>();
        var split = data.Split(';');
        System.Enum.TryParse(split[0], out Type);
        for (int i = 1; i < split.Length; i++)
        {
            Params.Add(split[i]);
        }

        InitData();
    }

    private void InitData()
    {
        switch (Type)
        {
            case ELootType.Null:
                Data = NullData.Null;
                break;
            case ELootType.Equipment:
                Data = new EquipmentData(Params);
                break;
            case ELootType.BuffCard:
                Data = new BuffCardRewardData();
                break;
            case ELootType.Resource:
                Data = new ResourceData(Params);
                var res = Data as ResourceData;
                if (res.Resource == EResource.EquipmentRdFragment || (res.Resource >= EResource.MainWpFragment && res.Resource <= EResource.BootFragment))
                {
                    Type = ELootType.Fragment;
                }
                if (res.Resource == EResource.HeroRdFragment || (res.Resource >= EResource.NormalHero && res.Resource <= EResource.EvilHero))
                {
                    Type = ELootType.HeroFragment;
                }
                if (res.Resource == EResource.HeroStoneRdFragment || (res.Resource >= EResource.BaseStone && res.Resource <= EResource.LightStone))
                {
                    Type = ELootType.HeroStone;
                }
                break;
            case ELootType.Exp:
                Data = new ExpData(Params);
                break;
            case ELootType.Hero:
                Data = new HeroData(Params);
                break;
            case ELootType.Require:
                Data = new RequireData(Params);
                break;
        }
    }

    public override string ToString()
    {
        return Data.ToString();
    }
    public LootParams Clone()
    {
        return new LootParams { Type = Type, Data = Data.CloneData() };
    }
}