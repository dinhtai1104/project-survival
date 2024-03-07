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
    public class GearTable : DataTable<string, GearEntity>
    {
        private Dictionary<string, Dictionary<ERarity, GearEntity>> m_PerkGearEquipment;

        public override void GetDatabase()
        {
            m_PerkGearEquipment = new Dictionary<string, Dictionary<ERarity, GearEntity>>();
        }

        private void Get(BGEntity e)
        {
            var gear = new GearEntity(e);
            Dictionary.Add(gear.IdGear, gear);

            if (!m_PerkGearEquipment.ContainsKey(gear.IdEquipment))
            {
                m_PerkGearEquipment.Add(gear.IdEquipment, new Dictionary<ERarity, GearEntity>());
            }

            m_PerkGearEquipment[gear.IdEquipment].Add(gear.Rarity, gear);
        }

        public Dictionary<ERarity, GearEntity> GetRarityPerks(string idEqp)
        {
            return m_PerkGearEquipment[idEqp];
        }
    }
}
