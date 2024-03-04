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
        
        public override void GetDatabase()
        {
            DB_Dungeon.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
        }

        public class RowTemp
        {
            public int DungeonId;
            public int WaveId;
            public int LevelEnemy;

        }
    }
}
