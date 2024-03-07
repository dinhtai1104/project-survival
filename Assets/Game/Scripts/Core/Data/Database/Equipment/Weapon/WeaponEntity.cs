using Assets.Game.Scripts.Enums;
using BansheeGz.BGDatabase;
using Engine;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon
{
    [System.Serializable]
    public class WeaponEntity
    {
        public string IdEquipment => Equipment.Id;
        public EquipmentEntity Equipment;
        // Other properties
        public ERarity Rarity;

        public EDamageType DamageType;
        public EWeaponScaling WeaponScaling;

        public float Cooldown;
        public float Range;
        public float Velocity;
        public float MeleeWeaponReach;
        public int ProjectilesPerShot;

        public List<ModifierData> WeaponBaseStat;
        public List<ModifierData> WeaponUpgradeStat;
        public List<string> StatShowInUI;


        public WeaponEntity(BGEntity e)
        {
            var eqpId = e.Get<string>("IdEquipment");
            Equipment = DataManager.Base.Equipment.Get(eqpId);
            Cooldown = e.Get<float>("Cooldown");
            Range = e.Get<float>("Range");
            Velocity = e.Get<float>("Velocity");
            MeleeWeaponReach = e.Get<float>("MeleeWeaponReach");
            ProjectilesPerShot = e.Get<int>("ProjectilesPerShot");

            WeaponBaseStat = new List<ModifierData>();
            WeaponUpgradeStat = new List<ModifierData>();

            var weaponBaseStatData = e.Get<List<string>>("WpBaseStat");
            var weaponUpgradeStatData = e.Get<List<string>>("WpUpgradeStat");
            StatShowInUI = e.Get<List<string>>("StatUI");

            foreach (var stat in weaponBaseStatData)
            {
                var statSplit = stat.Split(';');
                var statB = new ModifierData(statSplit[0], new StatModifier(EStatMod.Flat, float.Parse(statSplit[1])));
                WeaponBaseStat.Add(statB);
            }

            foreach (var stat in weaponUpgradeStatData)
            {
                var statSplit = stat.Split(';');
                var statB = new ModifierData(statSplit[0], new StatModifier(EStatMod.Flat, float.Parse(statSplit[1])));
                WeaponUpgradeStat.Add(statB);
            }
        }
    }
}
