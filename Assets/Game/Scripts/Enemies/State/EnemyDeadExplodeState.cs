using com.mec;
using UnityEngine;
using MoreMountains.Feedbacks;
using Pool;
using Engine;
using AIState;

namespace States
{
    public class EnemyDeadExplodeState : ActorDeathState
    {
        [SerializeField] private BindGameConfig m_RadiusExplode = new BindGameConfig("[{0}]DeadExplodeRadius", 3);
        [SerializeField] private BindGameConfig m_DamageExplode = new BindGameConfig("[{0}]DeadExplodeDamage", 1.2f);
        [SerializeField] private float m_Delay = 2f;
        [SerializeField] private MMF_Player m_Feedback;
        [SerializeField] private GameObject m_Explode;

        [SerializeField] private DamageDealer m_DamageDealer = new DamageDealer();
        public override void Enter()
        {
            Clear();

            m_RadiusExplode.SetId(Actor.name);
            m_DamageDealer.Init(Actor.Stats);
            Actor.IsDead = true;
            m_Feedback?.PlayFeedbacks();
            Timing.CallDelayed(m_Delay, () =>
            {
                base.Enter();
            });
        }

        protected override void FireEvent()
        {
            base.FireEvent();
            var explode = PoolFactory.Spawn(m_Explode, Actor.CenterPosition, Quaternion.identity)
                .GetComponent<Explosion2D>();
            explode.Owner = Actor;
            explode.DamageDealer.CopyData(m_DamageDealer);
            explode.DamageDealer.DamageSource.Value *= m_DamageExplode.FloatValue;

            explode.TargetLayer = Actor.EnemyLayerMask;
            explode.Radius = m_RadiusExplode.FloatValue;
            
            explode.StartExplosion();
        }
    }
}
