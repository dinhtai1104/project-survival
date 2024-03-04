using UnityEngine;

namespace Engine
{
    public class SkillTask : Task, IStopHandler
    {
        [SerializeField] private int m_SkillId;
        [SerializeField] private bool m_InvokeCastingSkill;
        [SerializeField] private bool m_CooldownAtBegin;
        [SerializeField] private bool m_CooldownAtEnd;
        [SerializeField] private Skill m_Skill;
        [SerializeField] private Actor m_Actor;

        public Actor Caster
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
            if (m_Actor == null) m_Actor = GetComponentInParent<Actor>();
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