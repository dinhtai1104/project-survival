using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Assets.Game.Scripts.Utilities.Utilities.Scripts;
using Framework;
using System;
using UnityEngine;
namespace Engine.Weapon
{
    public class WeaponActor : ActorBase
    {
        private ActorBase m_Owner;
        private WeaponEntity m_WeaponEntity;


        [SerializeField] private Transform m_TriggerPoint;
        [SerializeField] private MoveFollowTarget m_TargetFollow;

        private Transform m_PlaceHolderWeapon;
        public Transform PlaceHolderWeapon => m_PlaceHolderWeapon;
        public ActorBase Owner => m_Owner;
        public Transform TriggerPoint => m_TriggerPoint;
        public WeaponEntity WpEntity => m_WeaponEntity;
        public ERarity Rarity => WpEntity.Rarity;

        private WeaponStatBridge m_BridgeStat;

        public void InitOwner(ActorBase owner)
        {
            m_Owner = owner;
        }

        public void InitWeapon(WeaponEntity weaponEntity)
        {
            m_WeaponEntity = weaponEntity;

            Stats.AddStat(StatKey.Damage, weaponEntity.Damage, 0);
            Stats.AddStat(StatKey.AttackRange, weaponEntity.RangeAttack);
            Stats.AddStat(StatKey.AttackSpeed, weaponEntity.AttackSpeed, -100);
            Stats.AddStat(StatKey.Knockback, weaponEntity.Knockback);
            Stats.AddStat(StatKey.Projectiles, weaponEntity.Projectiles, 1);
            Stats.AddStat(StatKey.CritChance, weaponEntity.CritChance, 0);
            Stats.AddStat(StatKey.CritDamage, 0.5f, 0);
            Stats.AddStat(StatKey.Velocity, weaponEntity.Velocity, 0);
            Stats.AddStat(StatKey.Pierce, 0);
            Stats.AddStat(StatKey.PierceReduce, 0);
            Stats.AddStat(StatKey.AngleZone, 0);

            foreach (var statOther in weaponEntity.OtherStats)
            {
                if (!Stats.HasStat(statOther.AttributeName))
                {
                    Stats.AddStat(statOther.AttributeName, statOther.Modifier.Value);
                    continue;
                }
                Stats.AddModifier(statOther.AttributeName, statOther.Modifier.Clone(), null);
            }

            Stats.CalculateStats();
        }

        public void SetPlaceHolder(Transform placeHolder)
        {
            m_PlaceHolderWeapon = placeHolder;
            //m_TargetFollow.SetTarget(placeHolder);
            Trans.SetParent(placeHolder, false);
            Trans.localPosition = Vector3.zero;
        }

        public void Active()
        {
            IsActivated = true;
            m_BridgeStat?.Dispose();
            m_BridgeStat = new WeaponStatBridge(this);
        }

#if DEVELOPMENT
        bool isstop = false;
        protected override void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.S))
            {
                isstop = !isstop;
            }
            if (isstop) return;
            base.Update();
        }
#endif
    }
}
