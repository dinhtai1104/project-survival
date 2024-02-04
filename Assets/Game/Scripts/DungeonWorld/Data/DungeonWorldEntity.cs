using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DungeonWorld.Data
{
    [Serializable]
    public class DungeonWorldEntity
    {
        public int Dungeon;
        public List<DungeonWorldStageEntity> Stages = new List<DungeonWorldStageEntity>();
        public void Add(DungeonWorldStageEntity stage)
        {
            Dungeon = stage.Dungeon;
            Stages.Add(stage);
        }

        public DungeonWorldStageEntity Get(int stage)
        {
            return Stages.Find(t => t.Stage == stage);
        }
    }
}
