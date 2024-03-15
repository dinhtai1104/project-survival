using BansheeGz.BGDatabase;
using GameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Equipment
{
    [System.Serializable]
    public class EquipmentTable : DataTable<string, EquipmentEntity>
    {
        public override void GetDatabase()
        {
            DB_Equipment.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var equip = new EquipmentEntity(e);
            Dictionary.Add(equip.IdEquipment, equip);
        }

        public EquipmentEntity GetRandomWeight()
        {
            return Dictionary.Values.ToList().RandomWeighted(out int dex);
        }
    }
}
