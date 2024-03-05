using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonWaveTable : DataTable<string, DungeonWaveEntity>
    {
        private List<DungeonEventWaveEnemy> ListWave = new List<DungeonEventWaveEnemy>();
        public override void GetDatabase()
        {
            ListWave.Clear();
            DB_DungeonWave.ForEachEntity(e => Get(e));

            var groupByWaveId = ListWave.GroupBy(e => e.WaveId);
            foreach (var wave in groupByWaveId)
            {
                var entity = new DungeonWaveEntity
                {
                    WaveId = wave.Key
                };

                foreach (var waveE in wave)
                {
                    entity.EventEnemies.Add(waveE);
                }

                Dictionary.Add(wave.Key, entity);
            }
        }

        private void Get(BGEntity e)
        {
            var entity = new DungeonEventWaveEnemy(e);
            ListWave.Add(entity);
        }
    }
}
