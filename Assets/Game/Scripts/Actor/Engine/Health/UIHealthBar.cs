using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressBar;

namespace Engine
{
    public class UIHealthBar : UIHealthHud
    {
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private UIProgressBar m_HealthBar;
        [SerializeField] private bool m_AnimateBar = true;

        private static Camera MainCamera;
        private IHealth m_Health;
        private float m_CurrentPercentage;

        private void Awake()
        {
            if (MainCamera == null) MainCamera = Camera.main;

            if (m_Canvas == null)
            {
                m_Canvas = GetComponent<Canvas>();
            }

            if (m_Canvas != null)
            {
                m_Canvas.worldCamera = MainCamera;
            }
        }

        private void OnDestroy()
        {
        }

        private void OnEnable()
        {
            m_Canvas.enabled = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_Health != null)
            {
                m_Health.OnValueChanged -= OnValueChanged;
            }
        }

        public override void SetOwner(Actor actor)
        {
            base.SetOwner(actor);
            m_Health = actor.Health;
            m_Health.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(IHealth health)
        {
            if (health.HealthPercentage < m_CurrentPercentage)
            {
            }

            m_CurrentPercentage = health.HealthPercentage;
            m_HealthBar.SetValue(health.HealthPercentage, m_AnimateBar);

            if (health.CurrentHealth <= 0f)
            {
                m_Canvas.enabled = false;
            }
        }

        private IEnumerator Hide()
        {
            m_Canvas.enabled = true;
            yield return 1.0f;
            m_Canvas.enabled = false;
        }
    }
}