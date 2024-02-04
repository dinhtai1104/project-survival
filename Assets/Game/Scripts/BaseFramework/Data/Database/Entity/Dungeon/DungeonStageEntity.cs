using BansheeGz.BGDatabase;
using System.Collections.Generic;

[System.Serializable]
public class DungeonStageEntity
{
    public string Stage;
    /// <summary>
    /// Random Room (if has more than 1 room => pick random room in this stage)
    /// </summary>
    public List<DungeonRoomEntity> RoomRandom;
    public List<LootParams> RewardStage;
    public int LevelEnemyStage;
    public DungeonStageEntity()
    {
        RoomRandom = new List<DungeonRoomEntity>();
    }

    public DungeonStageEntity(BGEntity e)
    {
        RoomRandom = new List<DungeonRoomEntity>();
        Stage = e.Get<string>("Stage");
        var listId = e.Get<List<int>>("Room");
        LevelEnemyStage = e.Get<int>("EnemyLevel");

        foreach (var room in listId)
        {
            var roomEntity = DataManager.Base.Room.GetRoom(room);
            RoomRandom.Add(roomEntity);
        }
    }

    public DungeonRoomEntity GetRoom()
    {

        return RoomRandom.Random();
    }
    public DungeonRoomEntity GetRoom(int room)
    {
        if (room == -1)
        {
            return GetRoom();
        }
        Logger.Log("GET ROOM:" + room);
        return RoomRandom[room];
    }
}