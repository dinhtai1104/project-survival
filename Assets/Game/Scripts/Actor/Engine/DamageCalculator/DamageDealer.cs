using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    [Serializable, HideLabel, FoldoutGroup("Damage Dealer")]
    public class DamageDealer : IDamageDealer
    {
        [SerializeField] private DamageSource m_Source;

        [SerializeField] private bool m_ApplyEffect = true;

        public DamageSource DamageSource => m_Source;

        public Actor Owner { set; get; }

        public DamageDealer()
        {
            m_Source = new DamageSource();
        }

        public void Init(IStatGroup stat)
        {
            stat.AddListener(StatKey.Damage, OnUpdateBaseDamage);
        }

        public void Release(IStatGroup stat)
        {
            stat.RemoveListener(StatKey.Damage, OnUpdateBaseDamage);
        }

        private void OnUpdateBaseDamage(float value)
        {
            m_Source.Value = value;
        }

        public HitResult DealDamage(Actor attacker, Actor defender)
        {
            if (defender.IsDead)
            {
                return HitResult.FailedResult;
            }

           

            HitResult hitResult = defender.DamageCalculator.CalculateDamage(defender, attacker, m_Source);

            return hitResult;
        }

        public void CopyData(DamageDealer damageDealer)
        {
            m_Source.Value = damageDealer.DamageSource.Value;
            m_Source.Type = damageDealer.DamageSource.Type;
            m_Source.DamageHealthPercentage = damageDealer.DamageSource.DamageHealthPercentage;
            m_Source.CannotEvade = damageDealer.DamageSource.CannotEvade;
            m_Source.CannotHurt = damageDealer.DamageSource.CannotHurt;
            Owner = damageDealer.Owner;
        }
    }
}