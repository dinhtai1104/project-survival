﻿using Assets.Game.Scripts.Events;
using Core;
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class DamageCalculator : MonoBehaviour, IDamageCalculator
    {
        private readonly Dictionary<EDamageType, bool> m_ImmuneDict = new Dictionary<EDamageType, bool>();
        private readonly DamageDealer m_ReflectDamageDealer = new DamageDealer();
        private const float MinDamage = 1f;
        private readonly TemporaryModifier m_AttackerTempMods = new TemporaryModifier();
        private readonly TemporaryModifier m_DefenderTempMods = new TemporaryModifier();

        public ActorBase Owner { get; private set; }

        public void Init(ActorBase actor)
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

        public HitResult CalculateDamage(ActorBase defender, ActorBase attacker, DamageSource source)
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
            damage = CalculatePhysicalDamage(defender, attacker, source, out success, out crit, out hurt, out evade, out block);

            // Clear temporary mods
            m_AttackerTempMods.Clear(attacker.Stats);
            m_DefenderTempMods.Clear(defender.Stats);


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
            GameArchitecture.GetService<IEventMgrService>().Fire(this, new DamageAfterHitEventArgs(attacker, defender, source, hitResult));
            //GameCore.Event.Fire(this, DamageAfterHitEventArgs.Create(attacker, defender, source, hitResult));

            if (lastHit)
            {
                // FireEvent LastHit
                GameArchitecture.GetService<IEventMgrService>().Fire(this, new LastHitEventArgs(attacker, defender));
                //GameCore.Event.Fire(this, DamageLastHitEventArgs.Create(attacker, defender, source, hitResult));
            }

            return hitResult;
        }

        private float CalculateRawDamage(DamageSource source, ActorBase defender)
        {
            return source.Value + defender.Health.MaxHealth * source.DamageHealthPercentage;
        }

        private float CalculatePhysicalDamage(ActorBase defender, ActorBase attacker, DamageSource source, out bool success,
            out bool crit, out bool hurt,
            out bool evade, out bool block)
        {
            success = true;
            crit = false;
            hurt = false;
            evade = false;
            block = false;

            var def = defender.Stats;
            m_DefenderTempMods.ApplyModifiers(def);

            var atk = attacker.Stats;
            m_AttackerTempMods.ApplyModifiers(atk);

            float critChance = atk.GetValue(StatKey.CritChance);
            crit = MathUtils.RandomBoolean(critChance);

            // Calculate base damage
            float damage = source.Value;

            if (crit && !source.CannotCrit)
            {
                damage *= atk.GetValue(StatKey.CritDamage) + 1;
            }

            if (defender.Tagger.HasTag(Tags.BossTag))
            {
                damage *= atk.GetValue(StatKey.BossDamage) + 1;
            }

            // Calculate Armor
            damage = FormulaHelper.CalculateDamageArmorTaken(attacker, defender, damage);

            damage += defender.Health != null ? defender.Health.MaxHealth * source.DamageHealthPercentage : 0f;
            if (damage < MinDamage) damage = MinDamage;
            return damage;
        }

        private float CalculateMagicDamage(ActorBase defender, ActorBase attacker, DamageSource source)
        {
            var def = defender.Stats;
            var atk = attacker.Stats;
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