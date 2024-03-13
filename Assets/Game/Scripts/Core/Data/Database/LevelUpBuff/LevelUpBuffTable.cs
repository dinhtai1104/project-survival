using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.LevelUpBuff
{
    [System.Serializable]
    public class LevelUpBuffTable : DataTable<int, LevelUpBuffEntity>
    {
        public override void GetDatabase()
        {
            DB_LevelUp.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var entity = new LevelUpBuffEntity(e);
            Dictionary.Add(entity.Id, entity);
        }
    }
}
