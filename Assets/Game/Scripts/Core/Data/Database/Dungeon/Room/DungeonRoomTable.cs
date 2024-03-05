using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonRoomTable : DataTable<string, List<DungeonRoomEntity>>
    {
        private List<DungeonEventEnemyWaveInfo> EventWaveInfo = new List<DungeonEventEnemyWaveInfo>();
        private Dictionary<int, DungeonRoomEntity> DungeonRooms = new Dictionary<int, DungeonRoomEntity>();
        public override void GetDatabase()
        {
            EventWaveInfo.Clear();
            DungeonRooms.Clear();
            DB_DungeonRoom.ForEachEntity(e => Get(e));

            var groupByRoomId = EventWaveInfo.GroupBy(e => e.RoomId);

            foreach (var room in groupByRoomId)
            {
                var roomEntity = new DungeonRoomEntity();
                roomEntity.RoomId = room.Key;

                foreach (var @event in room)
                {
                    roomEntity.TagRoom = @event.TagRoom;
                    roomEntity.EventSpawn.Add(@event);
                }

                DungeonRooms.Add(room.Key, roomEntity);

                // Add Room to tag
                if (!Dictionary.ContainsKey(roomEntity.TagRoom))
                {
                    Dictionary.Add(roomEntity.TagRoom, new List<DungeonRoomEntity>());
                }
                Dictionary[roomEntity.TagRoom].Add(roomEntity);
            }
        }

        private void Get(BGEntity e)
        {
            var @event = new DungeonEventEnemyWaveInfo(e);
            EventWaveInfo.Add(@event);
        }
        public DungeonRoomEntity GetRoomTag(string roomTag)
        {
            return Get(roomTag).Random();   
        }
    }
}
