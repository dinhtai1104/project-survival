using System.Collections;
using System.Collections.Generic;
using Core;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    [DisallowMultipleComponent]
    public class Health : BaseHealth
    {
        [SerializeField, MinValue(0f)] private float m_MaxHealth;
        [SerializeField] private float m_CurrentHealth;
        [SerializeField] private bool m_Invincible;
        [SerializeField] private UnityEvent m_OnHealthZero;
        [SerializeField] private UnityEvent m_OnHealthFull;

        private bool m_IsZero;
        private bool m_IsFull;

        public override float CurrentHealth
        {
            set
            {
                if (float.IsNaN(value)) return;

                m_CurrentHealth = Mathf.Clamp(value, 0, m_MaxHealth);

                InvokeChangeValue();

                if (m_CurrentHealth <= 0f)
                {
                    if (Initialized && !m_IsZero)
                    {
                        m_OnHealthZero?.Invoke();
                        Architecture.Get<EventMgr>().Fire(this, new ActorDieEventArgs(Owner));
                    }

                    m_IsZero = true;
                    m_CurrentHealth = 0f;
                }
                else if (m_CurrentHealth >= m_MaxHealth)
                {
                    if (Initialized && !m_IsFull)
                    {
                        m_OnHealthFull?.Invoke();
                        Architecture.Get<EventMgr>().Fire(this, new HealthFullEventEventArgs(Owner));
                        //GameCore.Event.Fire(this, HealthFullEventEventArgs.Create(Owner));
                    }

                    m_IsFull = true;
                    m_CurrentHealth = m_MaxHealth;
                }
                else if (m_CurrentHealth > 0f && m_CurrentHealth < m_MaxHealth)
                {
                    m_IsFull = false;
                    m_IsZero = false;
                }
            }
            get { return m_CurrentHealth; }
        }

        public override float MaxHealth
        {
            set
            {
                if (value > m_MaxHealth)
                {
                    float diff = value - m_MaxHealth;
                    m_MaxHealth = value;
                    CurrentHealth += diff;
                }
                else
                {
                    m_MaxHealth = value;
                }
            }
            get { return m_MaxHealth; }
        }

        public override float MinHealth { get; set; }

        [ShowInInspector, ReadOnly]
        public override float HealthPercentage
        {
            get { return m_CurrentHealth / m_MaxHealth; }
        }

        public override bool Invincible
        {
            get { return m_Invincible; }
            set { m_Invincible = value; }
        }

        public override void Reset()
        {
            m_IsZero = false;
            m_IsFull = false;
        }

        public override void Damage(float damage, EDamageType type)
        {
            if (Invincible || CurrentHealth <= MinHealth)
            {
                return;
            }

            DamageHealth(DamageShieldEDamageType(type, damage));
            InvokeDamageEvent(damage, type);
        }

        public override void Healing(float amount)
        {
            CurrentHealth += amount;
        }

        private void DamageHealth(float damage)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - damage, MinHealth);
        }
    }
}