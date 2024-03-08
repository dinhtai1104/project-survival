using BansheeGz.BGDatabase;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon
{
    [System.Serializable]
    public class WeaponTable : DataTable<string, Dictionary<ERarity, WeaponEntity>>
    {
        public override void GetDatabase()
        {
            DB_Weapon.ForEachEntity(e => Get(e));
        }
        
        private void Get(BGEntity e)
        {
            var entity = new WeaponEntity(e);
            if (!Dictionary.ContainsKey(entity.IdEquipment))
            {
                Dictionary.Add(entity.IdEquipment, new Dictionary<ERarity, WeaponEntity>());
            }

            Dictionary[entity.IdEquipment].Add(entity.Rarity, entity);
        }

        public WeaponEntity GetWeapon(string idEq, ERarity rarity)
        {
            return Dictionary[idEq][rarity];
        }
    }
}
