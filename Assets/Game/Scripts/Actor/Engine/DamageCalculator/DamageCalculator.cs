using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Spine.Unity.Examples.BasicPlatformerController;

namespace Engine
{
    public class DamageCalculator : MonoBehaviour, IDamageCalculator
    {
        private readonly Dictionary<EDamageType, bool> m_ImmuneDict = new Dictionary<EDamageType, bool>();
        private readonly DamageDealer m_ReflectDamageDealer = new DamageDealer();
        private const float MinDamage = 1f;
        private readonly TemporaryModifier m_AttackerTempMods = new TemporaryModifier();
        private readonly TemporaryModifier m_DefenderTempMods = new TemporaryModifier();

        public Actor Owner { get; private set; }

        public void Init(Actor actor)
        {
            Owner = actor;
        }

        public void SetImmuneDamageType(EDamageType type, bool immune)
        {
            if (!m_ImmuneDict.ContainsKey(type))
            {
                m_ImmuneDict.Add(type, false);
            }

            m_ImmuneDict[type] = immune;
        }

        public bool IsImmuneDamageType(EDamageType type)
        {
            if (m_ImmuneDict.ContainsKey(type)) return m_ImmuneDict[type];
            m_ImmuneDict.Add(type, false);
            return false;
        }

        public HitResult CalculateDamage(Actor defender, Actor attacker, DamageSource source)
        {
            if (defender.Health.Invincible)
            {
                return HitResult.InvincibleHitResult;
            }

            var success = true;
            var crit = false;
            var lastHit = false;
            var hurt = false;
            var evade = false;
            var block = false;

            float damage;
            bool immune = IsImmuneDamageType(source.Type);

            if (immune)
            {
                // FireEvent Hit Immune

                return HitResult.InvincibleHitResult;
            }

            // FireEvent BeforeHit
            //GameCore.Event.Fire(this, DamageBeforeHitEventArgs.Create(attacker, defender, source));

            switch (source.Type)
            {
                case EDamageType.PHYSICAL:
                    damage = CalculatePhysicalDamage(defender, attacker, source, out success, out crit, out hurt, out evade,
                        out block);
                    break;
                case EDamageType.MAGIC:
                    damage = CalculateMagicDamage(defender, attacker, source);
                    break;
                case EDamageType.RAW:
                    damage = CalculateRawDamage(source, defender);
                    break;
                default:
                    damage = defender.Health.MaxHealth * 0.01f;
                    break;
            }

            // Clear temporary mods
            m_AttackerTempMods.Clear(attacker.Stat);
            m_DefenderTempMods.Clear(defender.Stat);


            if (damage > 0f)
            {
                defender.Health.Damage(damage, source.Type);

                if (!defender.IsDead && defender.Health.CurrentHealth <= 0f)
                {
                    defender.IsDead = true;
                    lastHit = true;
                }
            }

            var hitResult = new HitResult(success, false, crit, lastHit, hurt, evade, block, damage, source.Type);

            //FireEvent After Hit
            //GameCore.Event.Fire(this, DamageAfterHitEventArgs.Create(attacker, defender, source, hitResult));

            if (lastHit)
            {
                // FireEvent LastHit
                //GameCore.Event.Fire(this, DamageLastHitEventArgs.Create(attacker, defender, source, hitResult));
            }

            return hitResult;
        }

        private float CalculateRawDamage(DamageSource source, Actor defender)
        {
            return source.Value + defender.Health.MaxHealth * source.DamageHealthPercentage;
        }

        private float CalculatePhysicalDamage(Actor defender, Actor attacker, DamageSource source, out bool success,
            out bool crit, out bool hurt,
            out bool evade, out bool block)
        {
            success = true;
            crit = false;
            hurt = false;
            evade = false;
            block = false;

            var def = defender.Stat;
            m_DefenderTempMods.ApplyModifiers(def);

            var atk = attacker.Stat;
            m_AttackerTempMods.ApplyModifiers(atk);


            // Calculate base damage
            float damage = source.Value;

            // Reflect damage to attacker. Cannot reflect Reflect and Overtime damage

            damage += defender.Health != null ? defender.Health.MaxHealth * source.DamageHealthPercentage : 0f;
            if (damage < MinDamage) damage = MinDamage;
            return damage;
        }

        private float CalculateMagicDamage(Actor defender, Actor attacker, DamageSource source)
        {
            var def = defender.Stat;
            var atk = attacker.Stat;
            m_DefenderTempMods.ApplyModifiers(def);

            float damage = source.Value;

            damage += defender.Health != null ? defender.Health.MaxHealth * source.DamageHealthPercentage : 0f;
            if (damage < MinDamage) damage = MinDamage;
            return damage;
        }

        public void AddAttackerTemporaryModifier(string modName, StatModifier mod)
        {
            m_AttackerTempMods.AddModifier(modName, mod);
        }

        public void AddDefenderTemporaryModifier(string modName, StatModifier mod)
        {
            m_DefenderTempMods.AddModifier(modName, mod);
        }

        #region Private classes

        private class TemporaryModifier
        {
            private readonly Dictionary<string, StatModifier> m_TempDict;

            public TemporaryModifier()
            {
                m_TempDict = new Dictionary<string, StatModifier>();
            }

            public void AddModifier(string name, StatModifier mod)
            {
                if (!m_TempDict.ContainsKey(name))
                {
                    m_TempDict.Add(name, mod);
                }
            }

            public void ApplyModifiers(IStatGroup stat)
            {
                foreach (var key in m_TempDict.Keys)
                {
                    stat.AddModifier(key, m_TempDict[key], this);
                }
            }

            public void Clear(IStatGroup stat)
            {
                stat.RemoveModifiersFromSource(this);
                m_TempDict.Clear();
            }
        }

        #endregion
    }
}