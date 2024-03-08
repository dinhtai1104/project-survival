using BansheeGz.BGDatabase;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Equipment.Gear
{
    [System.Serializable]
    public class GearPerkTable : DataTable<string, GearPerkEntity>
    {
        private Dictionary<string, Dictionary<ERarity, GearPerkEntity>> m_PerkGearEquipment;

        public override void GetDatabase()
        {
            m_PerkGearEquipment = new Dictionary<string, Dictionary<ERarity, GearPerkEntity>>();
            DB_GearPerk.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var gear = new GearPerkEntity(e);
            Dictionary.Add(gear.IdGear, gear);

            if (!m_PerkGearEquipment.ContainsKey(gear.IdEquipment))
            {
                m_PerkGearEquipment.Add(gear.IdEquipment, new Dictionary<ERarity, GearPerkEntity>());
            }

            m_PerkGearEquipment[gear.IdEquipment].Add(gear.Rarity, gear);
        }

        public Dictionary<ERarity, GearPerkEntity> GetRarityPerks(string idEqp)
        {
            if (m_PerkGearEquipment.ContainsKey(idEqp))
            {
                return m_PerkGearEquipment[idEqp];
            }
            return new Dictionary<ERarity, GearPerkEntity>();
        }
    }
}
