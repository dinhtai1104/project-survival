using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon.EnemyEvent
{
    public class EnemiesEventTable : DataTable<string, EnemiesEventEntity>
    {
        private Dictionary<string, List<EnemiesEventEntity>> WaveEnemiesEvent;
        public override void GetDatabase()
        {
            WaveEnemiesEvent = new Dictionary<string, List<EnemiesEventEntity>>();
            DB_DungeonWaveEvent.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var entity = new EnemiesEventEntity(e);
            Dictionary.Add(entity.EventId, entity);

            if (!WaveEnemiesEvent.ContainsKey(entity.WaveId))
            {
                WaveEnemiesEvent.Add(entity.WaveId, new List<EnemiesEventEntity>());
            }
            WaveEnemiesEvent[entity.WaveId].Add(entity);
        }

        public List<EnemiesEventEntity> GetEvents(string waveId)
        {
            if (WaveEnemiesEvent.ContainsKey(waveId))
            {
                return WaveEnemiesEvent[waveId];
            }
            return new List<EnemiesEventEntity>();
        }
    }
}
