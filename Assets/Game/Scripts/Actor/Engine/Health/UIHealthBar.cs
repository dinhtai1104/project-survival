using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

namespace Engine
{
    public class UIHealthBar : UIProgressBar
    {
        private IHealth m_Health;
        private float m_CurrentPercentage;


        protected void OnDisable()
        {
            if (m_Health != null)
            {
                m_Health.OnValueChanged -= OnValueChanged;
            }
        }

        public void Init(Actor actor)
        {
            m_Health = actor.Health;
            m_Health.OnValueChanged += OnValueChanged;
            OnValueChanged(m_Health);
        }

        private void OnValueChanged(IHealth health)
        {
            if (health.HealthPercentage < m_CurrentPercentage)
            {
            }

            m_CurrentPercentage = health.HealthPercentage;
            SetValue(health.HealthPercentage, false);
        }
    }
}