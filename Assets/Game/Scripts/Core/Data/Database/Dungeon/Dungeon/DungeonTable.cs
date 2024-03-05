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
            var roomDatabase = DataManager.Base.DungeonRoom;
            var dg = Get(dungeonId);
            var newDg = new DungeonEntity { DungeonId = dungeonId };
            foreach (var wave in dg.Waves)
            {
                var newWave = new WaveInDungeonEntity
                {
                    DungeonId = dungeonId,
                    Length = wave.Length,
                    LevelEnemy = wave.LevelEnemy,
                };
                var waveInfo = new DungeonWaveEntity();
                waveInfo.WaveId = wave.WaveId;

                foreach (var info in wave.WaveInfo.EventEnemies)
                {
                    var dEvent = new DungeonEventWaveEnemy
                    {
                        WaveId = info.WaveId,
                        Room = roomDatabase.GetRoomTag(info.TagRoom),
                        TagRoom = info.TagRoom,
                        Time = info.Time,
                    };
                    waveInfo.EventEnemies.Add(dEvent);
                }
                newWave.WaveInfo = waveInfo;
                newDg.Waves.Add(newWave);
            }
            return newDg;
        }
    }
}
