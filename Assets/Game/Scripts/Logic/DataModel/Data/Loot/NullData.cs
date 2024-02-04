using System.Collections.Generic;

[System.Serializable]
public class NullData : ILootData
{
    public double ValueLoot => 1;
    public static NullData Null = new NullData();
    public bool CanMergeData => true;
    public bool Add(ILootData data)
    {
        return true;
    }

    public List<LootParams> GetAllData()
    {
        return new List<LootParams>() {  };
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