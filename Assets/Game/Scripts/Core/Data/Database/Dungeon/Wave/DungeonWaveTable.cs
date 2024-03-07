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
        public override void GetDatabase()
        {
            DB_DungeonWave.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var entity = new DungeonWaveEntity(e);
            Dictionary.Add(entity.WaveId, entity);
        }

        public DungeonWaveEntity GetWave(string waveId)
        {
            var eventDatabase = DataManager.Base.EnemiesEvent;
            var wave = Dictionary[waveId].Clone();
            wave.AddEvent(eventDatabase.GetEvents(waveId));
            wave.DefaultChance = 100;

            foreach (var events in wave.EnemiesEvents)
            {
                wave.DefaultChance -= events.Chance;
            }
            return wave;
        }
    }
}
