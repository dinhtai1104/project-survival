using BansheeGz.BGDatabase;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Rarity
{
    [System.Serializable]
    public class RarityTable : DataTable<ERarity, RarityEntity>
    {
        public override void GetDatabase()
        {
        }

        private void Get(BGEntity e)
        {
            var entity = new RarityEntity(e);
            Dictionary.Add(entity.Rarity, entity);
        }
    }
}
