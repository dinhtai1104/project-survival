using Engine;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks.Enemy
{
    public class RunningToTargetTask : SkillTask
    {
        [SerializeField] private float m_TimeOut = 5f;
        private float m_DistanceRange;
        public override void Begin()
        {
            m_DistanceRange = Caster.Stats.GetValue(StatKey.AttackRange, 1f);
            base.Begin();
        }
        public override void Run()
        {
            base.Run();
            var target = Caster.TargetFinder.CurrentTarget;
        }
    }
}
