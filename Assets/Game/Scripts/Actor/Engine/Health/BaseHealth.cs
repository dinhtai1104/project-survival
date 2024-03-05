using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    public abstract class BaseHealth : MonoBehaviour, IHealth
    {
        public Actor Owner { get; private set; }
        public event Action<IHealth> OnValueChanged;
        public bool Initialized { get; set; }
        public abstract float CurrentHealth { get; set; }
        public abstract float MaxHealth { get; set; }
        public abstract float MinHealth { get; set; }
        public abstract float HealthPercentage { get; }
        public abstract bool Invincible { get; set; }

        public abstract void Damage(float damage, EDamageType type);

        public abstract void Healing(float amount);

        public abstract void Reset();

        private Dictionary<EDamageType, float> m_ShieldDict = new Dictionary<EDamageType, float>();
        private List<Action<float, EDamageType>> m_OnRecieveDamage = new List<Action<float, EDamageType>>();
        [Button]
        public void Kill()
        {
            CurrentHealth = 0;
        }

        public void Init(Actor actor)
        {
            Owner = actor;
            var damageTypes = Enum.GetValues(typeof(EDamageType)) as EDamageType[];
            if (damageTypes != null)
            {
                foreach (var t in damageTypes)
                {
                    if (!m_ShieldDict.ContainsKey(t))
                    {
                        m_ShieldDict.Add(t, 0f);
                    }
                }
            }
        }

        public void SubscribeReceiveDamageEvent(Action<float, EDamageType> callback)
        {
            m_OnRecieveDamage.Add(callback);
        }

        public void AddShieldEDamageType(EDamageType damageType, float amount)
        {
            m_ShieldDict[damageType] += amount;
        }

        public void SetShieldEDamageType(EDamageType damageType, float amount)
        {
            m_ShieldDict[damageType] = amount;
        }

        public float GetShieldEDamageType(EDamageType damageType)
        {
            return m_ShieldDict[damageType];
        }

        protected void InvokeChangeValue()
        {
            OnValueChanged?.Invoke(this);
        }

        protected float DamageShieldEDamageType(EDamageType damageType, float damage)
        {
            if (m_ShieldDict == null || !m_ShieldDict.ContainsKey(damageType))
            {
                return damage;
            }

            var currentShield = m_ShieldDict[damageType];

            if (currentShield <= 0f)
            {
                return damage;
            }

            if (currentShield > damage)
            {
                currentShield -= damage;
                m_ShieldDict[damageType] = currentShield;
                return 0f;
            }

            var remainingDamage = damage - currentShield;
            currentShield = 0f;
            m_ShieldDict[damageType] = currentShield;
            return remainingDamage;
        }

        protected void InvokeDamageEvent(float damage, EDamageType type)
        {
            foreach (var callback in m_OnRecieveDamage)
            {
                callback.Invoke(damage, type);
            }
        }
    }
}