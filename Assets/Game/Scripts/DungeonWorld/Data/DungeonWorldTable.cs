using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DungeonWorld.Data
{
    [Serializable]
    public class DungeonWorldTable : DataTable<int, DungeonWorldEntity>
    {
        private List<DungeonWorldStageEntity> allData = new List<DungeonWorldStageEntity>();
        public override void GetDatabase()
        {
            allData.Clear();
            DB_DungeonWorld.ForEachEntity(e => Get(e));
            var groupbyDungeon = allData.GroupBy(t => t.Dungeon).ToList();
            foreach (var dungeon in groupbyDungeon)
            {
                Dictionary.Add(dungeon.Key, new DungeonWorldEntity());
                foreach (var stage in dungeon.ToList())
                {
                    Dictionary[dungeon.Key].Add(stage);
                }
            }
        }


        private void Get(BGEntity e)
        {
            allData.Add(new DungeonWorldStageEntity(e));
        }
    }
}
