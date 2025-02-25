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
    public class PerkRarityEntity
    {
        public string IdEquipment;
        public string IdGear;
        public ERarity Rarity;
        public RarityPerkModifier PerkModifier;

        public PerkRarityEntity(BGEntity e)
        {
            IdEquipment = e.Get<string>("IdEquipment");
            IdGear = e.Get<string>("IdGear");
            Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
            Enum.TryParse(e.Get<string>("PerkType"), out ERarityPerk Perk);
            var perkModifierData = e.Get<string>("PerkStat");
            var modifier = new ModifierData(perkModifierData);
            PerkModifier = new RarityPerkModifier(Perk, modifier);
        }
    }
}
