using Engine;
using Pool;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks
{
    public class ColliderAttackTask : SkillTask
    {
        [SerializeField] private DamageDealer m_DamageDealer;
        [SerializeField] private GameObject m_HitEffect;
        [SerializeField] private Collider2D m_Collider;

        private float m_CurrentTime = 0;

        protected override void Awake()
        {
            base.Awake();
            m_DamageDealer = new DamageDealer();
            m_Collider.enabled = false;
        }
        public override void Begin()
        {
            base.Begin();
            m_DamageDealer.Init(Caster.Stats);
            m_Collider.enabled = true;
            m_CurrentTime = 0;
        }

        public override void End()
        {
            base.End();
            m_Collider.enabled = false;
        }
        public override void Interrupt()
        {
            base.Interrupt();
            m_Collider.enabled = false;
        }

        public void TriggerEnter2D(Collider2D collision)
        {
            if (Caster == null) return;
            var actor = collision.GetComponent<Engine.ActorBase>();
            if (actor == null) return;

            if (m_HitEffect)
            {
                PoolFactory.Spawn(m_HitEffect, actor.CenterPosition, Quaternion.identity);
            }
            m_DamageDealer.DealDamage(Caster, actor);
        } 
    }
}
