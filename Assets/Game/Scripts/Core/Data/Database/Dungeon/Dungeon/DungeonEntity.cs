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
        public DungeonWaveEntity WaveInfo;
        public string WaveId;
        public int DungeonId;
        public int LevelEnemy;
        public int Length;

        public WaveInDungeonEntity()
        {
        }

        public WaveInDungeonEntity(BGEntity e)
        {
            WaveId = e.Get<string>("WaveId");

            var waveDatabase = DataManager.Base.DungeonWave;

            WaveInfo = waveDatabase.Get(WaveId);
            DungeonId = e.Get<int>("DungeonId");
            LevelEnemy = e.Get<int>("LevelEnemy");
            Length = e.Get<int>("Length");
        }
    }
}
