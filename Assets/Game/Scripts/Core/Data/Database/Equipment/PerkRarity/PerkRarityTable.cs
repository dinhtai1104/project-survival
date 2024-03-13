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
    public class PerkRarityTable : DataTable<string, PerkRarityEntity>
    {
        private Dictionary<string, Dictionary<ERarity, PerkRarityEntity>> m_PerkGearEquipment;

        public override void GetDatabase()
        {
            m_PerkGearEquipment = new Dictionary<string, Dictionary<ERarity, PerkRarityEntity>>();
            DB_PerkRarity.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var gear = new PerkRarityEntity(e);
            Dictionary.Add(gear.IdGear, gear);

            if (!m_PerkGearEquipment.ContainsKey(gear.IdEquipment))
            {
                m_PerkGearEquipment.Add(gear.IdEquipment, new Dictionary<ERarity, PerkRarityEntity>());
            }

            m_PerkGearEquipment[gear.IdEquipment].Add(gear.Rarity, gear);
        }

        public Dictionary<ERarity, PerkRarityEntity> GetRarityPerks(string idEqp)
        {
            if (m_PerkGearEquipment.ContainsKey(idEqp))
            {
                return m_PerkGearEquipment[idEqp];
            }
            return new Dictionary<ERarity, PerkRarityEntity>();
        }
    }
}
