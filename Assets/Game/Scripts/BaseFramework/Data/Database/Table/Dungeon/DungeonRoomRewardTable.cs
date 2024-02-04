using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DungeonRoomRewardTable : DataTable<int, DungeonRoomRewardEntity>
{
    private int lastRoom = -1;
    private int lastIndex = -1;

    private List<DungeonRoomRewardRow> allRewards;
    public override void GetDatabase()
    {
        allRewards = new List<DungeonRoomRewardRow>();
        lastRoom = -1;
        lastIndex = -1;
        DB_DungeonRoomReward.ForEachEntity(e => Get(e));

        // Group by room then group by index
        var groupByRoom = allRewards.GroupBy(t =>
        {
            return t.Room;
        }).ToList();

        foreach (var gr in groupByRoom)
        {
            var roomId = gr.Key;
            Dictionary.Add(roomId, new DungeonRoomRewardEntity());
            var allRewardInRoom = gr.ToList();
            var groupByIndex = allRewardInRoom.GroupBy(t => t.Index).ToList();

            foreach (var reIndex in groupByIndex)
            {
                var index = reIndex.Key;
                var allReward = reIndex.ToList();

                var groupReward = new DungeonRoomRewardGroup();
                foreach (var r in allReward)
                {
                    groupReward.Add(r);
                }
                Dictionary[roomId].AddReward(groupReward);
            }
        }
    }

    private void Get(BGEntity e)
    {
        var rewardRow = new DungeonRoomRewardRow(e);
        allRewards.Add(rewardRow);
    }
}