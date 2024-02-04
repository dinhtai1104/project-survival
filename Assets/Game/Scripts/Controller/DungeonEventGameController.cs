using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Level;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEventGameController : BattleGameController
{
    private DungeonEventSessionSave session => currentSessionSave as DungeonEventSessionSave;
    public EDungeonEvent EventType;
    private DungeonEventSave eventSave
    {
        get => dungeonSave as DungeonEventSave;
        set
        {
            dungeonSave = value;
        }
    }
    private DungeonEventConfigEntity config;
    public override void LoadMap()
    {
        Dungeon = session.CurrentDungeon;
        EventType = session.Type;
        session.JoinDungeon(Dungeon);
        Stage = session.CurrentStage;
        eventSave = DataManager.Save.DungeonEvent.Saves[session.Type];
        config = DataManager.Base.DungeonEventConfig.Get(eventSave.Type)[session.CurrentDungeon];

        // if not cached before last play => create new cached map
        if (session.MemoryMap == null)
        {
            // cached memory map
            var mapMemory = new DungeonSessionMemoryMap();
            var listRoomId = new List<int>();

            // get dungeon data
            var dungeon = DataManager.Base.DungeonEventTables[session.Type].GetDungeon(0);
            dungeonSpawnCurrent = new DungeonEntity(0);
            foreach (var stages in dungeon.Stages)
            {
                var newStage = new DungeonStageEntity();
                var allRoomId = new List<int>();
                foreach (var rm in stages.Item1.RoomRandom)
                {
                    allRoomId.Add(rm.RoomId);
                }
                int roomId = allRoomId[0];
                if (allRoomId.Count == 1 && listRoomId.Contains(allRoomId[0]))
                {
                    listRoomId.Add(roomId);
                }
                else
                {
                    roomId = allRoomId.Random(listRoomId);
                    listRoomId.Add(roomId);
                }
                var room = DataManager.Base.Room.GetRoom(roomId);

                room.StageLinked = stages.Item1;
                newStage.RoomRandom.Add(room);
                newStage.RewardStage = stages.Item1.RewardStage;


                newStage.LevelEnemyStage = stages.Item1.LevelEnemyStage;
                dungeonSpawnCurrent.AddStage(newStage, stages.Item2);
            }
            mapMemory.SetIdRooms(listRoomId);
            currentSessionSave.SetMemoryMap(mapMemory);
        }
        else
        {
            // Load map from cached
            var dungeon = DataManager.Base.DungeonEventTables[session.Type].GetDungeon(0);
            dungeonSpawnCurrent = new DungeonEntity(0);
            int stageCurrent = 0;
            foreach (var roomId in currentSessionSave.MemoryMap.roomId)
            {
                var stages = dungeon.GetStage(stageCurrent);
                var newStage = new DungeonStageEntity();
                var room = DataManager.Base.Room.GetRoom(roomId);
                room.StageLinked = stages;
                newStage.RoomRandom.Add(room);
                newStage.RewardStage = stages.RewardStage;
                newStage.LevelEnemyStage = stages.LevelEnemyStage;

                dungeonSpawnCurrent.AddStage(newStage, dungeon.GetBuffStage(stageCurrent++));
            }
        }
    }
    public override async UniTask Initialize()
    {
        await base.Initialize();
        if (session.lootData.Count > 0) return;
        session.AddLoot(new LootParams(ELootType.Resource, config.StartReward));
    }
    public override async UniTask PrepareLevel(int room = -1)
    {
        await base.PrepareLevel(room);
    }
    protected override void SetStageReady()
    {
        onStageReady?.Invoke((int)gameModeData.gameMode,Dungeon, Stage, EventType);

    }
    protected override async UniTask SetUpBackGround(int dungeon, ERoomType roomType)
    {
        BackGround backGround = null;
        string backGroundId = gameModeData.GetBackGround((int)EventType, roomType).RuntimeKey.ToString();
             
        backGround = await levelBuilder.SetUpBackGround(backGroundId);
    }
    protected override Color GetWorldColor()
    {
        return gameModeData.GetWorldColor((int)EventType);

    }
    protected override void OnStageFinish(bool buffPortal)
    {
        base.OnStageFinish(buffPortal);
        session.AddLoot(new LootParams(ELootType.Resource, config.IncreaseReward));
    }
}