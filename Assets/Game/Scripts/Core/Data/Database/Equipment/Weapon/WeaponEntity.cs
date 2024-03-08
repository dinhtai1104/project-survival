using Assets.Game.Scripts.Enums;
using BansheeGz.BGDatabase;
using Engine;
using ExtensionKit;
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
        public string IdEquipment => Equipment.IdEquipment;
        public EquipmentEntity Equipment;
        // Other properties
        public ERarity Rarity;

        public EDamageType DamageType;
        public List<EWeaponScaling> WeaponScaling;

        public float AttackSpeed;
        public float Damage;
        public float CritChance;
        public float Knockback;
        public float RangeAttack;
        public int Projectiles;
        public float Velocity;
        public int Price;
        public int Unlock;

        public List<ModifierData> OtherStats;

        public List<string> StatShowInUI;

        public WeaponEntity()
        {
            WeaponScaling = new List<EWeaponScaling>();
            OtherStats = new List<ModifierData>();
        }

        public WeaponEntity(BGEntity e) : this()
        {
            Enum.TryParse(e.Get<string>("Rarity"), out Rarity);

            var eqpId = e.Get<string>("IdEquipment");
            Equipment = DataManager.Base.Equipment.Get(eqpId);
            Damage = e.Get<float>("Damage");
            AttackSpeed = e.Get<float>("AttackSpeed");
            CritChance = e.Get<float>("CritChance");
            RangeAttack = e.Get<float>("RangeAttack");
            Velocity = e.Get<float>("Velocity");
            Knockback = e.Get<float>("Knockback");
            Projectiles = e.Get<int>("Projectiles");
            Unlock = e.Get<int>("Unlock");
            StatShowInUI = e.Get<List<string>>("StatUI");
            Price = e.Get<int>("Price");


            var otherStat = e.Get<List<string>>("OtherStat");
            if (otherStat.IsNotNull())
            {
                foreach (var stat in otherStat)
                {
                    var statSplit = stat.Trim().Split(';');
                    var statB = new ModifierData(statSplit[0], new StatModifier(EStatMod.Flat, float.Parse(statSplit[1])));
                    OtherStats.Add(statB);
                }
            }
        }
    }
}
