using Engine;
using Engine.State.Common;
using RVO;
using System;
using UnityEngine;

namespace Engine.State.Unit
{
    public class UnitChaseState : BaseActorState
    {
        [SerializeField] private string m_AnimationChase;
        private Vector3 m_LastPos;
        private System.Random m_random = new System.Random();

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

            if (Math.Abs(m_AnimationTimeScale - m_CacheAnimationTimeScale) > 0)
            {
                m_AnimationTimeScale = Actor.Stats.GetValue(StatKey.Speed) / Actor.Stats.GetBaseValue(StatKey.Speed);
                Actor.Animation.TimeScale = m_AnimationTimeScale;
            }

            var target = Actor.TargetFinder.CurrentTarget;
            if (target != null)
            {
                Actor.Animation.EnsurePlay(0, m_AnimationChase, true);
                if (Actor.RVO.Id >= 0)
                {
                    Vector2RVO pos = Simulator.Instance.getAgentPosition(Actor.RVO.Id);
                    var position = new Vector3(pos.x(), pos.y());
                    var dir = Actor.CenterPosition - target.CenterPosition;

                    Actor.Trans.position = position;

                    //if (dir.magnitude < Actor.Stats.GetValue(StatKey.AttackRange))
                    //{
                    //    if (dir.magnitude < Actor.Stats.GetValue(StatKey.AttackRange) * 0.7f)
                    //    {
                    //        Actor.Movement.MoveDirection(dir.normalized);
                    //    }
                    //    else
                    //    {
                    //        ToIdleState();
                    //    }
                    //}
                    //else
                    //{
                    //    Actor.Movement.MoveTo(position);
                    //}
                }
                else
                {
                    Actor.RVO.ReInit();
                }

                Vector2RVO goalVector = new Vector2RVO(target.CenterPosition) - Simulator.Instance.getAgentPosition(Actor.RVO.Id);
                if (RVOMath.absSq(goalVector) > 1.0f)
                {
                    goalVector = RVOMath.normalize(goalVector);
                }
                Simulator.Instance.setAgentPrefVelocity(Actor.RVO.Id, goalVector/* * Actor.Stats.GetValue(StatKey.Speed)*/);


                /* Perturb a little to avoid deadlocks due to perfect symmetry. */
                float angle = (float)m_random.NextDouble() * 2.0f * (float)Math.PI;
                float dist = (float)m_random.NextDouble() * 0.0001f;

                Simulator.Instance.setAgentPrefVelocity(Actor.RVO.Id, Simulator.Instance.getAgentPrefVelocity(Actor.RVO.Id) +
                                                             dist *
                                                             new Vector2RVO((float)Math.Cos(angle), (float)Math.Sin(angle)));
            }
            else
            {
                //ToIdleState();
            }

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
