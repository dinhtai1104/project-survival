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
        public override void GetDatabase()
        {
            DB_DungeonRoom.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var dungeonRoom = new DungeonRoomEntity(e);
            if (!Dictionary.ContainsKey(dungeonRoom.TagRoom))
            {
                Dictionary.Add(dungeonRoom.TagRoom, new List<DungeonRoomEntity>());
            }
            Dictionary[dungeonRoom.TagRoom].Add(dungeonRoom);
        }
        public DungeonRoomEntity GetRoomTag(string roomTag)
        {
            return Get(roomTag).Random();
        }
    }
}
