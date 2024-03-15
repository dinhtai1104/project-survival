using Assets.Game.Scripts.Enums;
using BansheeGz.BGDatabase;
using Framework;
using GameUtility;
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
        private Dictionary<string, WeaponEntity> m_WeaponBaseEntity;
        private Dictionary<EWeaponClass, List<WeaponEntity>> m_WeaponClass;
        public override void GetDatabase()
        {
            m_WeaponBaseEntity = new Dictionary<string, WeaponEntity>();
            m_WeaponClass = new Dictionary<EWeaponClass, List<WeaponEntity>>();
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
            if (!m_WeaponBaseEntity.ContainsKey(entity.IdEquipment))
            {
                m_WeaponBaseEntity.Add(entity.IdEquipment, entity);
            }

            if (!m_WeaponClass.ContainsKey(entity.WeaponClass))
            {
                m_WeaponClass.Add(entity.WeaponClass, new List<WeaponEntity>());
            }
            m_WeaponClass[entity.WeaponClass].Add(entity);
        }

        public WeaponEntity GetWeapon(string idEq, ERarity rarity)
        {
            return Dictionary[idEq][rarity];
        }

        public string GetRandomWeightByClass(EWeaponClass @class)
        {
            return m_WeaponClass[@class].RandomWeighted(out int x).IdEquipment;
        }
        public WeaponEntity GetRandomWeight()
        {
            return m_WeaponBaseEntity.Values.ToList().RandomWeighted(out int x);
        }
    }
}
