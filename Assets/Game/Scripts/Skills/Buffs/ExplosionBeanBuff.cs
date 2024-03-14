using Assets.Game.Scripts.Core.BuffHandler;
using Assets.Game.Scripts.Events;
using Core;
using Engine;
using Pool;
using UnityEngine;

namespace Assets.Game.Scripts.Buffs
{
    public class ExplosionBeanBuff : BuffGameObject
    {
        private float m_ChanceExplode;
        private float m_ExplodeRadius;
        private float m_HealPercentage;

        [SerializeField] private DamageDealer m_DamageDealer;
        [SerializeField] private GameObject m_ExplodePrefab;

        protected override void OnInit()
        {
            m_DamageDealer = new DamageDealer();
            base.OnInit();
            m_ChanceExplode = BuffData.GetValue(StatKey.Random, DataGame.Data.EModifierBuff.Skill);
            m_ExplodeRadius = BuffData.GetValue(StatKey.Range, DataGame.Data.EModifierBuff.Skill);
            m_HealPercentage = BuffData.GetValue(StatKey.Hp, DataGame.Data.EModifierBuff.Skill);
            Architecture.Get<EventMgr>().Subscribe<LastHitEventArgs>(LastHitEventHandler);

            m_DamageDealer.Init(Owner.Stats);
            m_DamageDealer.DamageSource.Value = 0;
            m_DamageDealer.DamageSource.ClearModifiers();
            m_DamageDealer.DamageSource.CannotCrit = true;


            m_DamageDealer.DamageSource.DamageHealthPercentage = m_HealPercentage;
        }
        protected override void OnExit()
        {
            base.OnExit();
            Architecture.Get<EventMgr>().Unsubscribe<LastHitEventArgs>(LastHitEventHandler);
        }

        private void LastHitEventHandler(object sender, IEventArgs e)
        {
            var evt = e as LastHitEventArgs;
            var deadActor = evt.m_Patient;
            var killerActor = evt.m_Killer;
            if (deadActor == null) return;

            if (!MathUtils.RollChance(m_ChanceExplode)) return;
            var pos = deadActor.CenterPosition;

            var explosion = PoolFactory.Spawn(m_ExplodePrefab, pos, Quaternion.identity).GetComponent<Explosion2D>();
            explosion.Owner = killerActor;
            explosion.DamageDealer?.CopyData(m_DamageDealer);
            explosion.Radius = m_ExplodeRadius;
            explosion.StartExplosion();
#if DEVELOPMENT
            Debug.Log($"Explode | Chance: {m_ChanceExplode} ^ Radius: {m_ExplodeRadius} ^ %HpDamage: {m_HealPercentage} | using by " + name);
#endif
        }
    }
}
