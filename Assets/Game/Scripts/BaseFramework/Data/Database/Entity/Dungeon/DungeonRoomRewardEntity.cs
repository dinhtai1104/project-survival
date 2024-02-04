using BansheeGz.BGDatabase;
using System.Collections.Generic;

[System.Serializable]
public class DungeonRoomRewardEntity
{
    public List<DungeonRoomRewardGroup> ListRewardRandomInRoom;

    public DungeonRoomRewardEntity() 
    {
        ListRewardRandomInRoom = new List<DungeonRoomRewardGroup>();
    }

    public void AddReward(DungeonRoomRewardGroup row)
    {
        ListRewardRandomInRoom.Add(row);
    }
}