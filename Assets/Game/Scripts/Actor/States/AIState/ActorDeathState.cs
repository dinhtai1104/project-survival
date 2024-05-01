using com.mec;
using Core;
using Engine;
using Events;
using ExtensionKit;
using MoreMountains.Feedbacks;
using Pool;
using RVO;
using Spine;
using System;
using UnityEngine;

namespace AIState
{
    public class ActorDeathState : BaseState
    {
        [SerializeField] private string m_Animation;
        [SerializeField] protected float m_Delay = 0f;
        [SerializeField] private MMF_Player m_Feedback;

        private bool isFiredEvent = false;
        private bool IgnoreDeathAnimation => m_Animation.IsNullOrEmpty();
        public override void Enter()
        {
            base.Enter();
            Clear();
            Actor.IsDead = true;
            m_Feedback?.PlayFeedbacks();

            Timing.CallDelayed(m_Delay, () =>
            {
                m_Feedback?.ResetFeedbacks();
                DeadAnimation();
            });
        }

        protected void Clear()
        {
            Actor.SkillCaster.InterruptAllSkills();
            isFiredEvent = false;
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

        protected void DeadAnimation()
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

        private void CompleteEvent(TrackEntry trackEntry)
        {
            if (trackEntry.TrackCompareAnimation(m_Animation))
            {
                FireEvent();
            }
        }

        public override void Exit()
        {
            base.Exit();
            Actor.Animation.UnsubcribeComplete(CompleteEvent);
            isFiredEvent = false;
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

        protected virtual void FireEvent()
        {
            isFiredEvent = true;
            PoolFactory.Despawn(Actor.gameObject);
            GameArchitecture.GetService<IEventMgrService>().Fire(this, new ActorDieEventArgs(Actor));
        }
    }
}
