using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonEntity
    {
        public int DungeonId;
        public List<WaveInDungeonEntity> Waves;

        public DungeonEntity()
        {
            Waves = new List<WaveInDungeonEntity>();
        }
    }

    [System.Serializable]
    public class WaveInDungeonEntity
    {
        public int DungeonId;
        public string WaveId;
        public float Frequency;
        public float RandomAdd;
        public float DelayStart;
        public int LevelEnemy;
        public int Length;

        public DungeonWaveEntity WaveEntity;

        public WaveInDungeonEntity()
        {
        }

        public WaveInDungeonEntity(BGEntity e)
        {
            DungeonId = e.Get<int>("DungeonId");
            WaveId = e.Get<string>("WaveId");
            LevelEnemy = e.Get<int>("LevelEnemy");
            Length = e.Get<int>("Length");
            Frequency = e.Get<float>("Frequency");
            RandomAdd = e.Get<float>("RandomAdd");
            DelayStart = e.Get<float>("DelayStart");
        }
    }
}
