using com.mec;
using UnityEngine;
using MoreMountains.Feedbacks;
using Pool;
using Engine;
using AIState;
using System;
using Spine;

namespace States
{
    public class EnemyDeadExplodeState : BaseState
    {
        [SerializeField] private string m_Animation;

        [SerializeField] private BindConfig m_RadiusExplode = new BindConfig("[{0}]DeadExplodeRadius", 3);
        [SerializeField] private BindConfig m_DamageExplode = new BindConfig("[{0}]DeadExplodeDamage", 1.2f);
        [SerializeField] private float m_Delay = 2f;
        [SerializeField] private MMF_Player m_Feedback;
        [SerializeField] private GameObject m_Explode;

        [SerializeField] private DamageDealer m_DamageDealer = new DamageDealer();
        public override void Enter() 
        {
            base.Enter();
            Clear();
            Actor.IsDead = true;

            m_RadiusExplode.SetId(Actor.name);
            m_DamageDealer.Init(Actor.Stats);
            Actor.IsDead = true;
            m_Feedback?.PlayFeedbacks();
            Timing.CallDelayed(m_Delay, () =>
            {
                m_Feedback?.ResetFeedbacks();
                DeadAnimation();
            });
        }

        private void DeadAnimation()
        {
            if (Actor.Animation.HasAnimation(m_Animation))
            {
                Actor.Animation.EnsurePlay(0, m_Animation, false, true);
                Actor.Animation.SubscribeComplete(CompleteEvent);
                Actor.Animation.Lock = true;
            }
            else
            {
                Actor.Animation.Lock = true;
                FireEvent();
            }
        }

        private void Clear()
        {
            Actor.SkillCaster.InterruptAllSkills();
            Actor.Animation.TimeScale = 1f;
            Actor.IsDead = true;

            // Ensure the zero heal event to be broadcasted
            Actor.Health.CurrentHealth = 0f; 
            Actor.Movement.IsMoving = false;
            Actor.Movement.LockMovement = true;
            Actor.SetActiveCollider(false);
            Actor.Input.Lock = true;
            Actor.SkillCaster.InterruptAllSkills();
            Actor.SkillCaster.IsLocked = true;
            Actor.SkillCaster.ResetAllSkills();
            Actor.Status.Lock = true;
            Actor.Status.ClearAllStatus();
        }
        public override void Exit()
        {
            base.Exit();
            Actor.Animation.UnsubcribeComplete(CompleteEvent);
            Actor.IsDead = false;
            // Ensure the zero heal event to be broadcasted
            Actor.Animation.Lock = false;
            Actor.Movement.LockMovement = false;
            Actor.SetActiveCollider(true);
            Actor.Input.Lock = false;
            Actor.SkillCaster.IsLocked = false;
            Actor.Status.Lock = false;
            Actor.Brain.Lock = false;
            Actor.Animation.Stop();
        }

        private void CompleteEvent(TrackEntry trackEntry)
        {
            if (trackEntry.TrackCompareAnimation(m_Animation))
            {
                FireEvent();
            }
        }

        protected void FireEvent()
        {
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
