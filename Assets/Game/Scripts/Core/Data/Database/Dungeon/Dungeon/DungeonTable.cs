using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonTable : DataTable<int, DungeonEntity>
    {
        private List<WaveInDungeonEntity> WaveInDungeon = new List<WaveInDungeonEntity>();
        public override void GetDatabase()
        {
            WaveInDungeon.Clear();
            DB_Dungeon.ForEachEntity(e => Get(e));

            var groupByDungeonId = WaveInDungeon.GroupBy(e => e.DungeonId);
            foreach (var dungeon in groupByDungeonId)
            {
                var entity = new DungeonEntity
                {
                    DungeonId = dungeon.Key
                };

                foreach (var wave in dungeon)
                {
                    entity.Waves.Add(wave);
                }

                Dictionary.Add(dungeon.Key, entity);
            }
        }

        private void Get(BGEntity e)
        {
            var waveInD = new WaveInDungeonEntity(e);
            WaveInDungeon.Add(waveInD);
        }


        public DungeonEntity CreateDungeon(int dungeonId)
        {
            var eventDatabase = DataManager.Base.EnemiesEvent;
            var waveDatabase = DataManager.Base.DungeonWave;
            var dg = Get(dungeonId);
            var newDg = new DungeonEntity { DungeonId = dungeonId };
            foreach (var wave in dg.Waves)
            {
                var waveInfo = waveDatabase.GetWave(wave.WaveId);
                var newWave = new WaveInDungeonEntity
                {
                    DungeonId = dungeonId,
                    Length = wave.Length,
                    LevelEnemy = wave.LevelEnemy,
                    WaveId = wave.WaveId,
                    Frequency = wave.Frequency,
                    DelayStart = wave.DelayStart,
                    RandomAdd = wave.RandomAdd,
                    WaveEntity = waveInfo,
                };
                newDg.Waves.Add(newWave);
            }
            return newDg;
        }
    }
}
