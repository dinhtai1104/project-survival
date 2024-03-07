using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon.EnemyEvent
{
    [System.Serializable]
    public class EnemiesEventEntity
    {
        public string EventId;
        public string WaveId;
        public string Enemy;
        public float Frequency;
        public int Max;
        public float Chance;
        public float Cluster;

        public EnemiesEventEntity(BGEntity e)
        {
            EventId = e.Get<string>("EventId");
            WaveId = e.Get<string>("WaveId");
            Enemy = e.Get<string>("Enemy");
            Max = e.Get<int>("Max");
            Chance = e.Get<float>("Chance");
            Cluster = e.Get<float>("Cluster");
            Frequency = e.Get<float>("Frequency");
        }
    }
}
