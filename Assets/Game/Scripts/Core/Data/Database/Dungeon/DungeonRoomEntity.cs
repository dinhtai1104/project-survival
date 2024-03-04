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
        public List<DungeonEventEnemyWaveInfo> EnemyRowInfo;
    }
}
