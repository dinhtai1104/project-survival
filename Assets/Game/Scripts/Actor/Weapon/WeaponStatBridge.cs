using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Weapon
{
    public class WeaponStatBridge : IDisposable
    {
        // Weapon
        private WeaponActor m_Weapon;
        private IStatGroup m_StatWeapon => m_Weapon.Stats;
        private WeaponEntity m_Entity => m_Weapon.WpEntity;
        // Owner
        private ActorBase m_Owner => m_Weapon.Owner;
        private IStatGroup m_StatOwner => m_Owner.Stats;

        // Modifiers
        private StatModifier m_DamageBonusModifier;
        private StatModifier m_MeleeDamageModifier;
        private StatModifier m_RangeDamageModifier;
        private StatModifier m_ElementDamageModifier;
        private StatModifier m_EngineeringDamageModifier;
        private StatModifier m_AttackSpeedModifier;
        private StatModifier m_CritChanceModifier;
        private StatModifier m_CritDamageModifier;
        private StatModifier m_BossDamageModifier;
        private StatModifier m_KnockbackModifier;



        private void CreateModifier()
        {
            var Owner = m_Owner;
            m_DamageBonusModifier = new StatModifier(EStatMod.PercentAdd, Owner.Stats.GetValue(StatKey.DamageBonus));
            m_MeleeDamageModifier = new StatModifier(EStatMod.Flat, Owner.Stats.GetValue(StatKey.MeleeDamage));
            m_RangeDamageModifier = new StatModifier(EStatMod.Flat, Owner.Stats.GetValue(StatKey.RangeDamage));
            m_ElementDamageModifier = new StatModifier(EStatMod.Flat, Owner.Stats.GetValue(StatKey.ElementDamage));
            m_EngineeringDamageModifier = new StatModifier(EStatMod.Flat, Owner.Stats.GetValue(StatKey.EngineeringDamage));
            m_AttackSpeedModifier = new StatModifier(EStatMod.PercentAdd, -Owner.Stats.GetValue(StatKey.AttackSpeed));
            m_CritChanceModifier = new StatModifier(EStatMod.Flat, Owner.Stats.GetValue(StatKey.CritChance));
            m_CritDamageModifier = new StatModifier(EStatMod.Flat, Owner.Stats.GetValue(StatKey.CritDamage));
            m_BossDamageModifier = new StatModifier(EStatMod.Flat, Owner.Stats.GetValue(StatKey.BossDamage));
            m_KnockbackModifier = new StatModifier(EStatMod.PercentAdd, Owner.Stats.GetValue(StatKey.Knockback));
        }

        public WeaponStatBridge(WeaponActor actor)
        {
            m_Weapon = actor;
            CreateModifier();
            SetupBridge();
        }

        private void SetupBridge()
        {
            m_StatOwner.AddListener(StatKey.DamageBonus, OnChangeDamageBonus);
            m_StatOwner.AddListener(StatKey.ElementDamage, OnChangeElementDamage);
            m_StatOwner.AddListener(StatKey.RangeDamage, OnChangeRangeDamage);
            m_StatOwner.AddListener(StatKey.MeleeDamage, OnChangeMeleeDamage);
            m_StatOwner.AddListener(StatKey.EngineeringDamage, OnChangeEngineeringDamage);
            m_StatOwner.AddListener(StatKey.AttackSpeed, OnChangeAttackSpeed);
            m_StatOwner.AddListener(StatKey.CritChance, OnChangeCritChance);
            m_StatOwner.AddListener(StatKey.CritDamage, OnChangeCritDamage);
            m_StatOwner.AddListener(StatKey.BossDamage, OnChangeBossDamage);
            m_StatOwner.AddListener(StatKey.Knockback, OnChangeKnockback);

            // Add Modifier
            m_StatWeapon.AddModifier(StatKey.Damage, m_DamageBonusModifier, this);

            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Element))
            {
                m_StatWeapon.AddModifier(StatKey.Damage, m_ElementDamageModifier, this);
            }
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Range))
            {
                m_StatWeapon.AddModifier(StatKey.Damage, m_RangeDamageModifier, this);
            }
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Melee))
            {
                m_StatWeapon.AddModifier(StatKey.Damage, m_MeleeDamageModifier, this);
            }
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Engineering))
            {
                m_StatWeapon.AddModifier(StatKey.Damage, m_EngineeringDamageModifier, this);
            }

            m_StatWeapon.AddModifier(StatKey.AttackSpeed, m_AttackSpeedModifier, this);
            m_StatWeapon.AddModifier(StatKey.CritChance, m_CritChanceModifier, this);
            m_StatWeapon.AddModifier(StatKey.CritDamage, m_CritDamageModifier, this);
            m_StatWeapon.AddModifier(StatKey.BossDamage, m_BossDamageModifier, this);
            m_StatWeapon.AddModifier(StatKey.Knockback, m_KnockbackModifier, this);
        }

        private void OnChangeKnockback(float statValue)
        {
            m_KnockbackModifier.Value = statValue;
        }

        private void OnChangeBossDamage(float statValue)
        {
            m_BossDamageModifier.Value = statValue;
        }

        private void OnChangeCritDamage(float value)
        {
            m_CritDamageModifier.Value = value;
        }

        private void OnChangeCritChance(float value)
        {
            m_CritChanceModifier.Value = value;
        }

        private void OnChangeAttackSpeed(float value)
        {
            m_AttackSpeedModifier.Value = -value;
        }

        private void OnChangeEngineeringDamage(float value)
        {
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Engineering))
            {
                m_EngineeringDamageModifier.Value = value;
            }
        }

        private void OnChangeMeleeDamage(float value)
        {
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Melee))
            {
                m_MeleeDamageModifier.Value = value;
            }
        }

        private void OnChangeRangeDamage(float value)
        {
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Range))
            {
                m_RangeDamageModifier.Value = value;
            }
        }

        private void OnChangeElementDamage(float value)
        {
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Element))
            {
                m_ElementDamageModifier.Value = value;
            }
        }

        private void OnChangeDamageBonus(float value)
        {
            m_DamageBonusModifier.Value = value;
        }

        public void Dispose()
        {
            m_StatOwner.RemoveListener(StatKey.DamageBonus, OnChangeDamageBonus);
            m_StatOwner.RemoveListener(StatKey.ElementDamage, OnChangeElementDamage);
            m_StatOwner.RemoveListener(StatKey.RangeDamage, OnChangeRangeDamage);
            m_StatOwner.RemoveListener(StatKey.MeleeDamage, OnChangeMeleeDamage);
            m_StatOwner.RemoveListener(StatKey.EngineeringDamage, OnChangeEngineeringDamage);
            m_StatOwner.RemoveListener(StatKey.AttackSpeed, OnChangeAttackSpeed);
            m_StatOwner.RemoveListener(StatKey.CritChance, OnChangeCritChance);
            m_StatOwner.RemoveListener(StatKey.CritDamage, OnChangeCritDamage);
            m_StatOwner.RemoveListener(StatKey.BossDamage, OnChangeBossDamage);
            m_StatOwner.RemoveListener(StatKey.Knockback, OnChangeKnockback);

            // Add Modifier
            m_StatWeapon.RemoveModifier(StatKey.Damage, m_DamageBonusModifier);

            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Element))
            {
                m_StatWeapon.RemoveModifier(StatKey.Damage, m_ElementDamageModifier);
            }
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Range))
            {
                m_StatWeapon.RemoveModifier(StatKey.Damage, m_RangeDamageModifier);
            }
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Melee))
            {
                m_StatWeapon.RemoveModifier(StatKey.Damage, m_MeleeDamageModifier);
            }
            if (m_Entity.IsWeaponContainsScaling(Assets.Game.Scripts.Enums.EWeaponScaling.Engineering))
            {
                m_StatWeapon.RemoveModifier(StatKey.Damage, m_EngineeringDamageModifier);
            }

            m_StatWeapon.RemoveModifier(StatKey.AttackSpeed, m_AttackSpeedModifier);
            m_StatWeapon.RemoveModifier(StatKey.CritChance, m_CritChanceModifier);
            m_StatWeapon.RemoveModifier(StatKey.CritDamage, m_CritDamageModifier);
            m_StatWeapon.RemoveModifier(StatKey.BossDamage, m_BossDamageModifier);
            m_StatWeapon.RemoveModifier(StatKey.Knockback, m_KnockbackModifier);
        }
    }
}
