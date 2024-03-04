using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonRoomEntity
    {
        public int RoomId;
        public string TagRoom;
        public DungeonEventEnemyWaveInfo EventSpawn;

        public DungeonRoomEntity(BGEntity e)
        {
            RoomId = e.Get<int>("RoomId");
            TagRoom = e.Get<string>("TagRoom");

            var eventWave = new DungeonEventEnemyWaveInfo(e);
            eventWave.RoomId = RoomId;
            eventWave.TagRoom = TagRoom;
            EventSpawn = eventWave;
        }
    }
}
