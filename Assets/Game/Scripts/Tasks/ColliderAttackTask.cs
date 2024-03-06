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
        [SerializeField] private float m_Duration = 2f;

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

        public override void Run()
        {
            base.Run();
            m_CurrentTime += Time.deltaTime;
            if (m_CurrentTime > m_Duration)
            {
                IsCompleted = true;
            }
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
            var actor = collision.GetComponent<Engine.Actor>();
            if (actor == null) return;

            if (m_HitEffect)
            {
                PoolManager.Instance.Spawn(m_HitEffect, actor.CenterPosition, Quaternion.identity);
            }
            m_DamageDealer.DealDamage(Caster, actor);
        }
    }
}
