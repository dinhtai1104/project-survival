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
    public class WeaponEntity : IWeightable
    {
        public string PrefabPath;
        public string IdEquipment => Equipment.IdEquipment;
        public EquipmentEntity Equipment;
        // Other properties
        public ERarity Rarity;

        public EDamageType DamageType;
        public List<EWeaponScaling> WeaponScaling;

        public EWeaponClass WeaponClass;

        public float AttackSpeed;
        public float Damage;
        public float CritChance;
        public float Knockback;
        public float RangeAttack;
        public int Projectiles;
        public float Velocity;
        public int Price;
        public int Unlock;

        public float Weight => Equipment.Weight;

        public List<AttributeStat> MainStats;
        public List<ModifierData> OtherStats;

        public List<string> StatShowInUI;

        public WeaponEntity()
        {
            WeaponScaling = new List<EWeaponScaling>();
            OtherStats = new List<ModifierData>();
            MainStats = new List<AttributeStat>();
        }

        public WeaponEntity(BGEntity e) : this()
        {
            Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
            Enum.TryParse(e.Get<string>("Class"), out WeaponClass);

            var eqpId = e.Get<string>("IdEquipment");
            PrefabPath = e.Get<string>("PrefabPath");
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

            SetMainStat();


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

            var weaponScaling = e.Get<List<string>>("WeaponScaling");
            if (weaponScaling.IsNotNull())
            {
                foreach (var ws in weaponScaling)
                {
                    this.WeaponScaling.Add((EWeaponScaling)Enum.Parse(typeof(EWeaponScaling), ws));
                }
            }
        }

        private void SetMainStat()
        {
            MainStats.Add(new AttributeStat(StatKey.Damage, new Stat(Damage)));
            MainStats.Add(new AttributeStat(StatKey.CritChance, new Stat(CritChance)));
            MainStats.Add(new AttributeStat(StatKey.Knockback, new Stat(Knockback)));
            MainStats.Add(new AttributeStat(StatKey.AttackRange, new Stat(RangeAttack)));
            MainStats.Add(new AttributeStat(StatKey.AttackSpeed, new Stat(AttackSpeed)));
        }

        public bool IsWeaponContainsScaling(EWeaponScaling scale)
        {
            return WeaponScaling.Contains(scale);
        }
     }
}
