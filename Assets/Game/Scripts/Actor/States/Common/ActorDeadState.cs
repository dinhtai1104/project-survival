using Core;
using Engine;
using Events;
using ExtensionKit;
using Pool;
using Spine;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Common
{
    public class ActorDeadState : BaseState
    {
        [SerializeField] private string m_Animation;
        private bool isFiredEvent = false;
        private bool IgnoreDeathAnimation => m_Animation.IsNullOrEmpty();
        public override void Enter()
        {
            base.Enter();
            Actor.SkillCaster.InterruptAllSkills();
            isFiredEvent = false;
            Actor.Animation.TimeScale = 1f;
            Actor.IsDead = true;
            Actor.Animation.EnsurePlay(0, m_Animation, false);
            Actor.Animation.SubscribeComplete(CompleteEvent);

                // Ensure the zero heal event to be broadcasted
            Actor.Health.CurrentHealth = 0f;
            Actor.Animation.Lock = true;
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

        private void CompleteEvent(TrackEntry trackEntry)
        {
            FireEvent();
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
        }

        private void FireEvent()
        {
            isFiredEvent = true;
            Architecture.Get<EventMgr>().Fire(this, new ActorDieEventArgs(Actor));
            PoolManager.Instance.Despawn(Actor.gameObject);
        }
    }
}
