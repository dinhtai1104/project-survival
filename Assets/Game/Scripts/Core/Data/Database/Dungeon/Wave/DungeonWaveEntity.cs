using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Dungeon
{
    [System.Serializable]
    public class DungeonWaveEntity
    {
        public string WaveId;
        public List<DungeonEventWaveEnemy> EventEnemies; // Event spawn enemy

        public DungeonWaveEntity()
        {
            EventEnemies = new List<DungeonEventWaveEnemy>();
        }
    }
}
