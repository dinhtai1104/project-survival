using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Framework;
using UnityEngine;
namespace Engine.Weapon
{
    public class WeaponActor : Actor
    {
        private Actor m_Owner;
        private WeaponEntity m_WeaponEntity;


        [SerializeField] private Transform m_TriggerPoint;

        private Transform m_PlaceHolderWeapon;
        public Transform PlaceHolderWeapon => m_PlaceHolderWeapon;
        public Actor Owner => m_Owner;
        public Transform TriggerPoint => m_TriggerPoint;
        public WeaponEntity WpEntity => m_WeaponEntity;
        public ERarity Rarity => WpEntity.Rarity;

        public void InitOwner(Actor owner)
        {
            m_Owner = owner;
        }

        public void InitWeapon(WeaponEntity weaponEntity)
        {
            m_WeaponEntity = weaponEntity;

            Stats.AddStat(StatKey.Damage, weaponEntity.Damage, 0);
            Stats.AddStat(StatKey.AttackRange, weaponEntity.RangeAttack);
            Stats.AddStat(StatKey.AttackSpeed, weaponEntity.AttackSpeed, 0.5f);
            Stats.AddStat(StatKey.Knockback, weaponEntity.Knockback);
            Stats.AddStat(StatKey.Projectiles, weaponEntity.Projectiles, 1);
            Stats.AddStat(StatKey.CritChance, weaponEntity.CritChance, 0);
            Stats.AddStat(StatKey.Velocity, weaponEntity.Velocity, 0);
            Stats.AddStat(StatKey.Pierce, 0);
            Stats.AddStat(StatKey.PierceReduce, 0);
            Stats.AddStat(StatKey.AngleZone, 0);

            foreach (var statOther in weaponEntity.OtherStats)
            {
                Stats.AddModifier(statOther.AttributeName, statOther.Modifier.Clone(), null);
            }

            Stats.CalculateStats();
        }

        public void SetPlaceHolder(Transform placeHolder)
        {
            m_PlaceHolderWeapon = placeHolder;

            Trans.SetParent(placeHolder, false);
            Trans.localPosition = Vector3.zero;
        }
    }
}
