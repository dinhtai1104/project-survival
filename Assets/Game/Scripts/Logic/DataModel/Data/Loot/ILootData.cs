using System.Collections.Generic;

public interface ILootData
{
    double ValueLoot { get; }
    bool CanMergeData { get; }
    bool Add(ILootData data);
    void Loot();
    List<LootParams> GetAllData();
    ILootData CloneData();
    string GetParams();
}