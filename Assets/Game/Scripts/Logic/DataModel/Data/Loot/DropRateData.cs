using System.Collections.Generic;

[System.Serializable]
public class DropRateData : ILootData
{
    public double ValueLoot => 1;
    public ELootType LootType;
    public float Rate;

    public bool CanMergeData => true;
    public DropRateData() { }
    public DropRateData(List<string> param) : base()
    {
        System.Enum.TryParse(param[0], out LootType);
        float.TryParse(param[1], out Rate);
    }
    public DropRateData(string data) : base()
    {
        var param = data.Split(';');

        System.Enum.TryParse(param[0], out LootType);
        float.TryParse(param[1], out Rate);
    }

    public List<LootParams> GetAllData()
    {
        return new List<LootParams>() { };
    }
    public bool Add(ILootData data)
    {
        var drop = data as DropRateData;
        if (drop != null)
        {
            Rate += (data as DropRateData).Rate;
            return true;
        }
        return false;
    }
    public void Loot()
    {
    }

    ILootData ILootData.CloneData()
    {
        return new NullData();
    }
    public string GetParams()
    {
        return $"";
    }
}