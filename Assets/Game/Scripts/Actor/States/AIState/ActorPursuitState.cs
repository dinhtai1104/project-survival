using Engine;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AIState
{
    public class ActorPursuitState : BaseState
    {
        [SerializeField] private string m_MovingAnimation;
        [SerializeField] private bool m_LoopAnimation = true;
        [SerializeField] private bool m_SyncTimeScale;

        private float m_Range;
        private bool m_ReachTarget;

        private ActorBase Caster => Actor;

        public override void Enter()
        {
            base.Enter();
            m_Range = Caster.Stats.GetValue(StatKey.AttackRange);
            m_ReachTarget = false;
        }

        public override void Exit()
        {
            base.Exit();
            Caster.Movement.IsMoving = false;
            if (m_SyncTimeScale)
            {
                Caster.Animation.TimeScale = 1f;
            }
        }

        public override void Execute()
        {
            base.Execute();
            if (Caster.Movement.LockMovement)
            {
                return;
            }

            var target = Caster.TargetFinder.CurrentTarget;
            if (target != null)
            {
                var targetPos = target.Trans.position;
                var casterPos = Caster.Trans.position;
                if (!m_ReachTarget)
                {
                    if (m_MovingAnimation.IsNotNullAndEmpty())
                    {
                        Caster.Animation.Play(0, m_MovingAnimation, m_LoopAnimation);
                        if (m_SyncTimeScale)
                        {
                            var stats = Caster.Stats;
                            Caster.Animation.TimeScale = stats.GetValue(StatKey.Speed) / stats.GetBaseValue(StatKey.Speed);
                        }
                    }

                    if (Caster.AI) Caster.Movement.LookAt(targetPos);

                    var diffVec = targetPos - casterPos;
                    var distX = diffVec.magnitude;
                    diffVec.z = 0f;

                    if (distX <= m_Range)
                    {
                        return;
                    }
                    else
                    {
                        Caster.Movement.MoveTo(targetPos);
                    }
                }
            }
        }
    }
}
