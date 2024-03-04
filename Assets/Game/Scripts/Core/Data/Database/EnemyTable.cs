using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database
{
    public class EnemyTable : DataTable<string, EnemyEntity>
    {
        public override void GetDatabase()
        {
            DB_Enemy.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var enemy = new EnemyEntity(e);
            Dictionary.Add(enemy.Id, enemy);
        }
    }
}
