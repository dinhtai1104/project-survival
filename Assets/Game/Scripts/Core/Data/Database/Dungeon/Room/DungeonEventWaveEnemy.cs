using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonEventWaveEnemy
    {
        public string WaveId;
        public string TagRoom;
        public float Time;
        public DungeonRoomEntity Room;

        public DungeonEventWaveEnemy()
        {
        }

        public DungeonEventWaveEnemy(BGEntity e) 
        {
            WaveId = e.Get<string>("WaveId");
            TagRoom = e.Get<string>("TagRoom");
            Time = e.Get<float>("Time");
        }
    }
}
