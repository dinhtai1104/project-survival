using Assets.Game.Scripts.Actor.States.Common;
using Engine;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Unit
{
    public class UnitChaseState : BaseActorState
    {
        [SerializeField] private string m_AnimationChase;
        private Vector3 m_LastPos;

        private float m_CacheAnimationTimeScale;
        private float m_AnimationTimeScale;

        public override void Enter()
        {
            base.Enter();

            Actor.SkillCaster.InterruptCurrentSkill();
            Actor.Animation.EnsurePlay(0, m_AnimationChase, true);

            m_CacheAnimationTimeScale = Actor.Animation.TimeScale;
            m_AnimationTimeScale = 0;
        }
        public override void Execute()
        {
            base.Execute();
            var target = Actor.TargetFinder.CurrentTarget;
            if (target != null)
            {
                Actor.Animation.EnsurePlay(0, m_AnimationChase, true);
                var dir = target.CenterPosition - Actor.CenterPosition;

                if (Math.Abs(m_AnimationTimeScale - m_CacheAnimationTimeScale) > 0)
                {
                    m_AnimationTimeScale = Actor.Stats.GetValue(StatKey.Speed) / Actor.Stats.GetBaseValue(StatKey.Speed);
                    Actor.Animation.TimeScale = m_AnimationTimeScale;
                }

                Actor.Movement.MoveDirection(dir.normalized);
            }
            else
            {
                //ToIdleState();
            }
            m_LastPos = Actor.CenterPosition;
        }
        protected override void ToIdleState()
        {
            Actor.Fsm.ChangeState<UnitIdleState>();
        }
        public override void Exit()
        {
            base.Exit();
            Actor.Animation.TimeScale = 1f;
        }
    }
}
