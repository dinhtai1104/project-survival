using UnityEngine;

namespace Engine
{
    public class SkillTask : Task, IStopHandler
    {
        [SerializeField] protected int m_SkillId;
        [SerializeField] protected bool m_InvokeCastingSkill;
        [SerializeField] protected bool m_CooldownAtBegin;
        [SerializeField] protected bool m_CooldownAtEnd;
        [SerializeField] protected Skill m_Skill;
        [SerializeField] protected ActorBase m_Actor;

        public ActorBase Caster
        {
            set { m_Actor = value; }
            get { return m_Actor; }
        }

        protected Skill Skill
        {
            get { return m_Skill; }
        }

        public int SkillId
        {
            get
            {
                if (Skill != null)
                {
                    return Skill.Id;
                }

                return m_SkillId;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_Actor == null) m_Actor = GetComponentInParent<ActorBase>();
            if (m_Skill == null) m_Skill = GetComponentInParent<Skill>();
        }

        public override void Begin()
        {
            base.Begin();
            if (Ignore) return;
            if (Skill != null)
            {
                if (Skill.InvokeCastingSkillManually && m_InvokeCastingSkill)
                {
                    Skill.InvokeCastingSkill();
                }

                if (Skill.StartCooldownManually && m_CooldownAtBegin && !m_CooldownAtEnd)
                {
                    Skill.StartCooldown();
                }
            }
        }

        public override void End()
        {
            base.End();
            if (Ignore) return;
            if (Skill != null && Skill.StartCooldownManually && m_CooldownAtEnd && !m_CooldownAtBegin)
            {
                Skill.StartCooldown();
            }
        }

        public virtual void Interrupt()
        {
        }

        public void OnStop()
        {
            if (Skill != null && !Skill.IsCooldown && Skill.StartCooldownManually &&
                (m_CooldownAtEnd || m_CooldownAtBegin))
            {
                Skill.StartCooldown();
            }

            IsCompleted = true;
            Interrupt();
        }
    }
}