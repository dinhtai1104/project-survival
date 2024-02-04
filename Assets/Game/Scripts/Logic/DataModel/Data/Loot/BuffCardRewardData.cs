using System.Collections.Generic;

[System.Serializable]
public class BuffCardRewardData : ILootData
{
    public double ValueLoot => 1;
    public bool CanMergeData => true;
    // BuffCard no param
    public bool Add(ILootData data)
    {
        return true;
    }

    public List<LootParams> GetAllData()
    {
        return new List<LootParams>() {  };
    }

    public string GetParams()
    {
        return "";
    }

    public void Loot()
    {
    }

    ILootData ILootData.CloneData()
    {
        return new NullData();
    }
}