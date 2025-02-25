using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public class Skill : MonoBehaviour, ISkill
    {
        [SerializeField] private int m_Id;
        [SerializeField, Range(0, 100)] private int m_Priority;
        [SerializeField] private bool m_IsLocked;
        [SerializeField] private bool m_Ignore;
        [SerializeField] private bool m_IsPassive;
        [SerializeField] private bool m_AutoCast;
        [SerializeField] private bool m_Interruptible = true;
        [SerializeField, BoxGroup("Cooldown")] private Stat m_Cooldown;
        [SerializeField, BoxGroup("Cooldown")] private Stat m_StartingCooldown;
        [SerializeField] private string m_CastingAnimation;
        [SerializeField] private bool m_LockOtherSkills = true;
        [SerializeField] private bool m_InvokeCastingSkillManually;
        [SerializeField] private bool m_StartCooldownManually = true;
        [SerializeField, Range(0f, 20f)] private float m_CastingRange;
        [SerializeField] private UnityEvent m_OnCasting;
        [SerializeField] private UnityEvent m_OnExit;
        [SerializeField] private UnityEvent m_OnInterrupt;

        private bool m_IsCooldown;
        private float m_CooldownTimer;
        private bool m_IsCooldownTimerRunning = true;
        private bool m_CastNoCooldown;
        private bool m_IsExecuting;

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public int Priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }

        public bool Ignore
        {
            get { return m_Ignore; }
            set { m_Ignore = value; }
        }

        public bool Interruptible
        {
            get { return m_Interruptible; }
            set { m_Interruptible = value; }
        }

        public float Cooldown
        {
            get { return m_Cooldown.Value; }
            set { m_Cooldown.BaseValue = value; m_Cooldown.RecalculateValue(); }
        }

        public bool AutoCast
        {
            get { return m_AutoCast; }
            set { m_AutoCast = value; }
        }

        public bool IsLocked
        {
            get { return m_IsLocked; }
            set { m_IsLocked = value; }
        }

        public bool IsCooldown
        {
            get { return m_IsCooldown; }
        }

        public bool InvokeCastingSkillManually
        {
            get { return m_InvokeCastingSkillManually; }
            set { m_InvokeCastingSkillManually = value; }
        }

        public bool StartCooldownManually
        {
            get { return m_StartCooldownManually; }
            set { m_StartCooldownManually = value; }
        }

        public bool LockOtherSkill
        {
            get { return m_LockOtherSkills; }
            set { m_LockOtherSkills = value; }
        }

        public bool IsCooldownTimerRunning
        {
            get { return m_IsCooldownTimerRunning; }
        }

        public float RemainingCooldownTime
        {
            get { return Mathf.Clamp(m_Cooldown.Value - m_CooldownTimer, 0f, m_Cooldown.Value); }
        }

        public bool CanCast
        {
            get
            {
                if (m_IsLocked || m_IsCooldown || m_IsExecuting) return false;
                return true;
            }
        }

        public float StartingCooldown
        {
            get { return m_StartingCooldown.Value; }
            set
            {
                m_StartingCooldown.BaseValue = value;
                m_StartingCooldown.RecalculateValue();
                m_IsCooldown = true;
                m_CooldownTimer = Mathf.Clamp(Cooldown - m_StartingCooldown.Value, 0f, Cooldown);
            }
        }

        public bool CastNoCooldown
        {
            get { return m_CastNoCooldown; }

            set
            {
                m_CastNoCooldown = value;
                if (m_IsCooldown && value)
                {
                    m_CooldownTimer = 0f;
                    m_IsCooldown = false;
                }
            }
        }

        public bool IsExecuting
        {
            get { return m_IsExecuting; }

            set
            {
                if (m_IsExecuting && !value)
                {
                    OnExit();
                }

                m_IsExecuting = value;
            }
        }

        public UnityEvent OnCastingEvent
        {
            get { return m_OnCasting; }
        }

        public UnityEvent OnExitEvent
        {
            get { return m_OnExit; }
        }

        public ActorBase Caster { get; private set; }

        public void Init(ActorBase actor)
        {
            Caster = actor;

            if (Id <= 0)
            {
                //if (Caster.Stat.HasStat(StatKey.AttackSpeed))
                //{
                //    m_Cooldown = Caster.Stat.GetValue(StatKey.AttackSpeed);
                //}
            }

            if (m_StartingCooldown.Value > 0)
            {
                StartCooldown();
            }

            OnInit();
        }

        public void OnUpdate()
        {
            if (m_IsCooldown)
            {
                if (m_IsCooldownTimerRunning)
                {
                    m_CooldownTimer += Time.deltaTime;
                }

                if (m_CooldownTimer >= m_Cooldown.Value)
                {
                    m_IsCooldown = false;
                    m_CooldownTimer = 0f;
                    OnCooldownComplete();
                }
            }

            if (IsExecuting)
            {
                OnExecuting();
            }
        }

        public void ResetCooldown()
        {
            m_IsCooldown = false;
            m_CooldownTimer = 0f;
        }

        public void FasterCooldown(float duration)
        {
            m_CooldownTimer = Mathf.Clamp(m_CooldownTimer + duration, 0f, Cooldown);
        }

        public void StartCooldown()
        {
            if (!m_CastNoCooldown)
            {
                m_IsCooldown = true;
                m_CooldownTimer = 0f;
            }

            m_CastNoCooldown = false;
        }

        public void Cast()
        {
            if (!CanCast) return;
            IsExecuting = true;
            OnCasting();
        }

        public void InvokeCastingSkill()
        {
            //GameCore.Event.Fire(this, CastSkillEventArgs.Create(Caster, this));
            m_OnCasting.Invoke();
        }

        public virtual void Reset()
        {
            IsExecuting = false;
            m_CooldownTimer = 0f;
            m_IsCooldown = false;
            m_CastNoCooldown = false;
        }

        public virtual void Stop()
        {
            //GameCore.Event.Fire(this, InterruptSkillEventArgs.Create(Caster, this));
            m_OnInterrupt.Invoke();
            IsExecuting = false;
        }

        public void SetActiveCooldownTimer(bool active)
        {
            m_IsCooldownTimerRunning = active;
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnCooldownComplete()
        {
        }

        protected virtual void OnCasting()
        {
        }

        protected virtual void OnExecuting()
        {
        }

        protected virtual void OnExit()
        {
            m_OnExit.Invoke();
        }

        public void SetManuallyCooldown(Stat cooldown)
        {
            this.m_Cooldown = cooldown;
        }

        public void SetManuallyStartCooldown(Stat cooldown)
        {
            this.m_StartingCooldown = cooldown;
        }
    }
}