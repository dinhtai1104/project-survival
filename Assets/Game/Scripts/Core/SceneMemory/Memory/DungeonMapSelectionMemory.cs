using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.SceneMemory.Memory
{
    public class DungeonMapSelectionMemory : ISceneMemory
    {
        public DungeonEntity DungeonEntity { get; }

        public DungeonMapSelectionMemory(DungeonEntity adventureEntity)
        {
            DungeonEntity = adventureEntity;
        }
    }
}
