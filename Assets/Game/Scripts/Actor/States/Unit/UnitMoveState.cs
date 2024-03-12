using Engine;
using Engine.State.Common;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine.State.Unit
{
    public class UnitMoveState : BaseActorState
    {
        [SerializeField] private string m_Animation;

        private float m_CacheAnimationTimeScale;
        private float m_AnimationTimeScale;

        public override void Enter()
        {
            base.Enter();
            Actor.SkillCaster.InterruptCurrentSkill();
            Actor.Animation.Play(0, m_Animation);
            m_CacheAnimationTimeScale = Actor.Animation.TimeScale;
            m_AnimationTimeScale = 0;
        }

        public override void Execute()
        {
            base.Execute();
            var stat = Actor.Stats;
            var movement = Actor.Movement;
            movement.MoveDirection(Vector2.right, stat.GetValue(StatKey.Speed));
            if (Math.Abs(m_AnimationTimeScale - m_CacheAnimationTimeScale) > 0)
            {
                m_AnimationTimeScale = Actor.Stats.GetValue(StatKey.Speed) / Actor.Stats.GetBaseValue(StatKey.Speed);
                Actor.Animation.TimeScale = m_AnimationTimeScale;
            }
        }
        public override void Exit()
        {
            base.Exit();
            Actor.Animation.TimeScale = 1f;
        }
    }
}
