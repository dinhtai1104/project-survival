using BansheeGz.BGDatabase;
using System;
using UnityEngine;

[System.Serializable]
public class RoomTable : DataTable<int, DungeonRoomEntity>
{
    public override void GetDatabase()
    {
        DB_DungeonRoom.ForEachEntity(e => Get(e));
        DB_DungeonEventGoldRoom.ForEachEntity(e => Get(e));
        DB_DungeonEventScrollRoom.ForEachEntity(e => Get(e));
        DB_DungeonEventStoneRoom.ForEachEntity(e => Get(e));
    }

    public DungeonRoomEntity GetRoom(int roomId)
    {
        try
        {
            return Dictionary[roomId];
        }
        catch (Exception ex)
        {
            Debug.Log("Error in room: " + roomId);
        }
        return null;
    }

    private void Get(BGEntity e)
    {
        int RoomId = e.Get<int>("Room");
        int WaveId = e.Get<int>("Wave");

        if (!Dictionary.ContainsKey(RoomId))
        {
            // Create new Room if not exist
            Dictionary.Add(RoomId, new DungeonRoomEntity(e));
        }

        var room = Dictionary[RoomId];
        var enemyData = new EnemyWaveEntity(e);
        if (!room.HasWave(WaveId))
        {
            var waveData = new DungeonWaveEntity(WaveId);
            waveData.RoomLink = Dictionary[RoomId];
            Dictionary[RoomId].AddWave(waveData);
            waveData.DelayToWave = e.Get<float>("DelayToWave");
        }
        // Add data enemy to wave
        Dictionary[RoomId].AddEnemyToWave(enemyData, WaveId);
    }
}
/*
 * One Zone has many wave => 1 Zone - n Wave
 * One Wave has many Enemy => 1 Wave - n Enemy
 * 
 * zone: List<Wave>
 * Wave: List<Enemy>, Map, Reward, DropRate, Difficult
 * 
 * 
 * **/