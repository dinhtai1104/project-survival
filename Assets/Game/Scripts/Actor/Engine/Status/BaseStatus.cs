using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Engine
{
    public class BaseStatus : MonoBehaviour, IStatus
    {
        [SerializeField] private Tagger m_Tagger;
        [SerializeField] private bool m_Permanent;
        [SerializeField] private bool m_Expirable = true;

        [SerializeField, InlineProperty, HideLabel, BoxGroup("Duration"), GUIColor(0.3f, 0.8f, 0.8f)]
        private Stat m_Duration;

        [SerializeField, BoxGroup("Stack Options")]
        private bool m_Override = true;

        [SerializeField, BoxGroup("Stack Options"), HideIf("m_Override")]
        private int m_MaxStack;

        private Transform m_Trans;
        private ActorBase m_Actor;
        private ActorBase m_Source;
        private bool m_IsExpired;
        private bool m_IsRunning;
        private float m_DurationTimer;

        public Transform Trans => m_Trans;

        public ITagger Tagger
        {
            get { return m_Tagger; }
        }

        public bool Permanent
        {
            get { return m_Permanent; }
        }

        public bool Expirable
        {
            get { return m_Expirable; }
        }

        public bool Stackable
        {
            get { return m_MaxStack > 0; }
        }

        public int MaxStack
        {
            get { return m_MaxStack; }
        }

        public bool Override
        {
            get { return m_Override; }
        }

        public bool IsExpired
        {
            get { return m_IsExpired; }
        }

        public bool IsRunning
        {
            get { return m_IsRunning; }
        }

        public Stat Duration
        {
            get { return m_Duration; }
        }

        [ShowInInspector, ReadOnly]
        public ActorBase Actor
        {
            get { return m_Actor; }
        }

        [ShowInInspector, ReadOnly]
        public ActorBase Source
        {
            get { return m_Source; }
        }

        protected virtual void Awake()
        {
            m_Trans = transform;
        }

        public void OnUpdate(float dt)
        {
            if (!m_IsRunning || m_IsExpired) return;

            Execute();

            if (Expirable)
            {
                m_DurationTimer -= dt;
                if (m_DurationTimer <= 0f)
                {
                    m_IsRunning = false;
                    m_IsExpired = true;
                }
            }
        }

        public virtual void Execute()
        {
        }

        public virtual void End()
        {
        }

        public virtual void Begin()
        {
            m_IsExpired = false;
            m_IsRunning = true;
            m_Duration.RecalculateValue();
            m_DurationTimer = m_Duration.Value;
        }

        public virtual void Cancel()
        {
            m_IsExpired = true;
            m_IsRunning = false;
        }

        public virtual void Stop()
        {
            Cancel();
            End();
        }

        public virtual void SetActor(ActorBase actor)
        {
            m_Actor = actor;
        }

        public virtual void SetSource(ActorBase source)
        {
            m_Source = source;
        }

        public virtual IStatus SetDuration(float duration)
        {
            m_Duration = new Stat(duration);
            return this;
        }

        public virtual IStatus SetModifierValue(float value)
        {
            return this;
        }
    }
}