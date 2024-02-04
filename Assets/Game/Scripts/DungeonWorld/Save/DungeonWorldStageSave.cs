using Assets.Game.Scripts.DungeonWorld.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DungeonWorld.Save
{
    [System.Serializable]
    public class DungeonWorldStageSave
    {
        public int Dungeon;
        public int Stage;
        public bool IsClaimed;

        public DungeonWorldStageSave() { }
        public DungeonWorldStageSave(int dungeon, int stage, bool isClaimed) : base()
        {
            Dungeon = dungeon;
            Stage = stage;
            IsClaimed = isClaimed;
        }

        public bool Fix(DungeonWorldStageEntity dungeonWorldStageEntity)
        {
            if (Stage != dungeonWorldStageEntity.Stage)
            {
                Stage = dungeonWorldStageEntity.Stage;
                return true;
            }
            return false;
        }
    }
}
