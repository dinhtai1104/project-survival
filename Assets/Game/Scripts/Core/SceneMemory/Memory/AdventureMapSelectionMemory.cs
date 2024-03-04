using Assets.Game.Scripts.Core.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.SceneMemory.Memory
{
    public class AdventureMapSelectionMemory : ISceneMemory
    {
        public AdventureEntity AdventureEntity { get; }

        public AdventureMapSelectionMemory(AdventureEntity adventureEntity)
        {
            AdventureEntity = adventureEntity;
        }
    }
}
