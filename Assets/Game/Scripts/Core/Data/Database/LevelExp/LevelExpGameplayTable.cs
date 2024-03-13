using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.LevelExp
{
    public class LevelExpGameplayTable : DataTable<int, long>
    {
        public override void GetDatabase()
        {
            DB_LevelExpGameplay.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var level = e.Get<int>("Level");
            var exp = e.Get<long>("ExpRequire");

            Dictionary.Add(level, exp);
        }
    }
}
