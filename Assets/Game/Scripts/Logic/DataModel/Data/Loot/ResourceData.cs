using System.Collections.Generic;
using System;

[System.Serializable]
public class ResourceData : ILootData
{
    public double ValueLoot => Value;
    public EResource Resource;
    public double Value;
    private List<string> rawData;

    public bool CanMergeData => true;
    public ResourceData() { }
    public ResourceData(List<string> param)
    {
        Enum.TryParse(param[0], out Resource);
        Value = double.Parse(param[1]);
        if (param.Count > 2)
        {
            var rd = long.Parse(param[2]);
            Value = (long)UnityEngine.Random.Range((int)Value, rd + 1);
        }
    }
    public ResourceData(string content)
    {
        var split = content.Split(';');
        Enum.TryParse(split[0], out Resource);
        Value = long.Parse(split[1]);

        if (split.Length > 2)
        {
            var rd = long.Parse(split[2]);
            Value = (long)UnityEngine.Random.Range((int)Value, rd + 1);
        }
    }

    public ResourceData(EResource type, double value)
    {
        Resource = type;
        Value = value;
    }

    public void Multiply(float value)
    {
        Value = (long)(Value * value);
    }
    public bool Add(ILootData data)
    {
        var resource = data as ResourceData;
        if (resource != null)
        {
            if (resource.Resource == Resource)
            {
                Value += resource.Value;
                if (Value <= 0) Value = 0;

                return true;
            }
        }
        return false;
    }
    public ResourceData Clone()
    {
        return new ResourceData { Resource = Resource, Value = Value };
    }
    public override string ToString()
    {
        return $"{Resource} {Value}";
    }
    public void Loot()
    {
        DataManager.Save.Resources.IncreaseResource(Clone());
    }

    public List<LootParams> GetAllData()
    {
        var ilist = new List<LootParams>();

        if (Resource == EResource.EquipmentRdFragment)
        {
            Dictionary<EResource, ResourceData> fragments = new Dictionary<EResource, ResourceData>();
            var types = new List<EResource>();
            for (EResource r = EResource.MainWpFragment; r <= EResource.BootFragment; r++)
            {
                types.Add(r);
            }

            for (int j = 0; j < Value; j++)
            {
                var type = types[(UnityEngine.Random.Range(0, types.Count))];
                if (!fragments.ContainsKey(type))
                {
                    fragments[type] = new ResourceData() { Resource = type };
                    ilist.Add(new LootParams(ELootType.Fragment, fragments[type]));
                }
                fragments[type].Value++;
            }
        }
        else if (Resource == EResource.HeroStoneRdFragment)
        {
            Dictionary<EResource, ResourceData> fragments = new Dictionary<EResource, ResourceData>();
            var types = new List<EResource>();
            for (EResource r = EResource.BaseStone; r <= EResource.LightStone; r++)
            {
                types.Add(r);
            }

            for (int j = 0; j < Value; j++)
            {
                var type = types[(UnityEngine.Random.Range(0, types.Count))];
                if (!fragments.ContainsKey(type))
                {
                    fragments[type] = new ResourceData() { Resource = type };
                    ilist.Add(new LootParams(ELootType.HeroStone, fragments[type]));
                }
                fragments[type].Value++;
            }
        }
        else if (Resource == EResource.HeroRdFragment)
        {
            Dictionary<EResource, ResourceData> fragments = new Dictionary<EResource, ResourceData>();
            var types = new List<EResource>();
            for (EResource r = EResource.NormalHero; r <= EResource.EvilHero; r++)
            {
                types.Add(r);
            }

            for (int j = 0; j < Value; j++)
            {
                var type = types[(UnityEngine.Random.Range(0, types.Count))];
                if (!fragments.ContainsKey(type))
                {
                    fragments[type] = new ResourceData() { Resource = type };
                    ilist.Add(new LootParams(ELootType.HeroFragment, fragments[type]));
                }
                fragments[type].Value++;
            }
        }
        else if (Resource == EResource.Exp)
        {
            ilist.Add(new LootParams(ELootType.Exp, new ExpData { Exp = (long)Value }));
        }
        else
        {
            var Type = ELootType.Resource;
            var ResourceType = this.Resource;
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
            ilist.Add(new LootParams(Type, this));
        }

        return ilist;
    }
    public string GetParams()
    {
        return $"Resource;{Resource};{Value}";
    }
    ILootData ILootData.CloneData()
    {
        return Clone();
    }
}