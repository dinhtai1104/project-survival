using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonEventEnemyWaveInfo
    {
        public int RoomId;
        public string TagRoom;

        public string Enemy; // enemy id
        public int IdZone; // zone enemy belong to
        public int Amount; // amount enemy spawn each time
        public float MinSpace; // Distance btw enemies
        public float SpawnRadius; // > 0 => spawn cluster

        public DungeonEventEnemyWaveInfo(BGEntity e)
        {
            Enemy = e.Get<string>("EnemyId");
            IdZone = e.Get<int>("ZoneId");
            Amount = e.Get<int>("Amount");
            MinSpace = e.Get<float>("MinSpace");
            SpawnRadius = e.Get<float>("SpawnRadius");
        }

        public bool IsCluster => SpawnRadius > 0;
    }
}
