using Assets.Game.Scripts.Gameplay.Data;
using BansheeGz.BGDatabase;
using Engine;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Equipment.Gear
{
    [System.Serializable]
    public class GearEntity
    {
        public string IdEquipment;
        public string IdGear;
        public ERarity Rarity;
        public RarityPerkModifier PerkModifier;

        public GearEntity(BGEntity e)
        {
            IdEquipment = e.Get<string>("IdEquipment");
            IdGear = e.Get<string>("IdGear");
            Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
            Enum.TryParse(e.Get<string>("PerkType"), out ERarityPerk Perk);
            var perkModifierData = e.Get<string>("PerkStat").Split(';');
            var statKey = perkModifierData[0];
            var modifier = new StatModifier(EStatMod.Flat, float.Parse(perkModifierData[1]));

            PerkModifier = new RarityPerkModifier(Perk, new ModifierData(statKey, modifier));
        }
    }
}
