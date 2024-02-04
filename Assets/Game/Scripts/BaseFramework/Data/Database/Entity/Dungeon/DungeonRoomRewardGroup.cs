using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonRoomRewardGroup : IWeightable
{
    public List<DungeonRoomRewardRow> Rewards = new List<DungeonRoomRewardRow>();
    public List<List<LootParams>> RewardsRaw = new List<List<LootParams>>();
    public int Chest;
    private float _weight = 0;
    public float Weight => _weight;

    public void Add(DungeonRoomRewardRow row)
    {
        Rewards.Add(row);
        RewardsRaw.Add(new List<LootParams> { row.Reward.Clone() });
        _weight = Mathf.Max(_weight, row.Weight);
        Chest = Mathf.Max(Chest, row.Chest);
    }
}