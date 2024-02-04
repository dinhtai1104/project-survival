using BansheeGz.BGDatabase;
using UnityEngine;

[System.Serializable]
public class DungeonRoomRewardRow
{
    public int Room;
    public int Index;
    public int Chest;
    public float Weight;
    public LootParams Reward;
    public DungeonRoomRewardRow(BGEntity e)
    {
        Room = e.Get<int>("Room");
        Index = e.Get<int>("Index");
        Chest = e.Get<int>("Chest");
        Weight = e.Get<float>("Weight");

        System.Enum.TryParse(e.Get<string>("Reward"), out EResource type);
        var random = e.Get<string>("Amount").Split('~');
        var value = Random.Range(int.Parse(random[0]), int.Parse(random[1]) + 1);
        var res = new ResourceData { Resource = type, Value = value }.GetAllData();
        Reward = new LootParams(res[0].Type, res[0].Data);
    }
}