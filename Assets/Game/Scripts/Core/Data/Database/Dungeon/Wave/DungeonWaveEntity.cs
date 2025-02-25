using Assets.Game.Scripts.Core.Data.Database.Dungeon.EnemyEvent;
using BansheeGz.BGDatabase;
using ExtensionKit;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonWaveEntity
    {
        public string WaveId;
        public int MaxInMap;
        public int DefaultAmount;
        public string DefaultEnemy;
        public float DefaultCluster;
        public float DefaultChance;
        public float CameraSize;
        private List<EnemiesEventEntity> m_EnemiesEvent;
        public List<EnemiesEventEntity> EnemiesEvents => m_EnemiesEvent;
        private BGEntity e;
        public DungeonWaveEntity(BGEntity e) : base()
        {
            this.e = e;
            WaveId = e.Get<string>("WaveId");
            MaxInMap = e.Get<int>("MaxInMap");
            DefaultAmount = e.Get<int>("DefaultAmount");
            DefaultEnemy = e.Get<string>("DefaultEnemy");
            DefaultChance = 100;
            DefaultCluster = e.Get<float>("DefaultCluster");
            CameraSize = e.Get<float>("CameraSize");
        }

        public DungeonWaveEntity()
        {
            m_EnemiesEvent = new List<EnemiesEventEntity>();
        }

        public void AddEvent(EnemiesEventEntity e)
        {
            m_EnemiesEvent.Add(e);
        }
        public void AddEvent(List<EnemiesEventEntity> e)
        {
            if (e.IsNullOrEmpty()) return;
            m_EnemiesEvent.AddRange(e);
        }

        public DungeonWaveEntity Clone()
        {
            return new DungeonWaveEntity
            {
                WaveId = WaveId,
                MaxInMap = MaxInMap,
                DefaultAmount = DefaultAmount,
                DefaultEnemy = DefaultEnemy,
                DefaultChance = DefaultChance,
                DefaultCluster = DefaultCluster,
            };
        }
    }
}
