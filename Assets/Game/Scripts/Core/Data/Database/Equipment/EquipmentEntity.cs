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
    public class EquipmentEntity
    {
        public string Id;
        public string Name;
        public EItemSlot SlotType;
        public Dictionary<ERarity, GearEntity> RarityPerks;
        public string BaseStatKey => BaseStat.AttributeName;
        public ModifierData BaseStat;

        public EquipmentEntity(BGEntity e)
        {
            Id = e.Get<string>("Id");
            Name = e.Get<string>("Name");
            Enum.TryParse(e.Get<string>("SlotType"), out SlotType);
            RarityPerks = DataManager.Base.RarityGear.GetRarityPerks(Id);

            var statData = e.Get<string>("Stat").Split(';');
            BaseStat = new ModifierData(statData[0], new StatModifier(EStatMod.Flat, float.Parse(statData[1])));
        }
    }
}
