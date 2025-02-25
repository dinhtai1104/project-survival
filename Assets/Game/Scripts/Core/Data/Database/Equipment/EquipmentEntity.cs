using Assets.Game.Scripts.Core.Data.Database.Equipment.Gear;
using BansheeGz.BGDatabase;
using Engine;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Equipment
{
    [System.Serializable]
    public class EquipmentEntity : IWeightable
    {
        public string IdEquipment;
        public EItemType ItemType;
        public Dictionary<ERarity, PerkRarityEntity> RarityPerks;
        public string BaseStatKey => BaseStat.AttributeName;
        public ModifierData BaseStat;
        private float weight = 0;
        public float Weight => weight;

        public EquipmentEntity(BGEntity e)
        {
            IdEquipment = e.Get<string>("IdEquipment");
            //Name = e.Get<string>("Name");
            Enum.TryParse(e.Get<string>("ItemType"), out ItemType);
            RarityPerks = DataManager.Base.GearPerk.GetRarityPerks(IdEquipment);

            var statData = e.Get<string>("StatName");
            BaseStat = new ModifierData(statData, new StatModifier(EStatMod.Flat, e.Get<float>("Stat")));

            weight = e.Get<float>("Weight");
        }
    }
}
