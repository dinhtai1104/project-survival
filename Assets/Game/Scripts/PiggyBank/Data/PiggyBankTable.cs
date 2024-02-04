using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.PiggyBank.Data
{
    [System.Serializable]
    public class PiggyBankTable : DataTable<int, PiggyBankEntity>
    {
        public override void GetDatabase()
        {
            DB_PiggyBank.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var entity = new PiggyBankEntity(e);
            Dictionary.Add(entity.Id, entity);
        }
    }
}
