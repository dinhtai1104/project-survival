using Engine;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks.Enemy
{
    public class DashToTargetTask : SkillTask
    {
        [SerializeField] private string m_AnimationDash;
        [SerializeField] private BindGameConfig m_TimeDash = new BindGameConfig("[{0}]TimeDash", 1);
        [SerializeField] private BindGameConfig m_SpeedMul = new BindGameConfig("[{0}]SpeedMul", 2f);

        private StatModifier modifier = new StatModifier();
        private Vector2 m_Direction;
        private float m_Time = 0;
        public override void Begin()
        {
            m_Time = 0;
            m_TimeDash.SetId(Caster.name);
            m_SpeedMul.SetId(Caster.name);
            Caster.Movement.LockFacing = true;
            base.Begin();

            var target = Caster.TargetFinder.CurrentTarget;
            if (target == null)
            {
                IsCompleted = true;
                Interrupt();
                return;
            }
            m_Direction = target.CenterPosition - Caster.CenterPosition;
        }

        public override void End()
        {
            Caster.Movement.LockFacing = false;
            base.End();
        }

        public override void Run()
        {
            base.Run();

            if (Caster.Movement.ReachBound)
            {
                IsCompleted = true;
                return;
            }
            m_Time += Time.deltaTime;
            if (m_Time > m_TimeDash.FloatValue)
            {
                IsCompleted = true;
                return;
            }
            Caster.Animation.EnsurePlay(0, m_AnimationDash, true);
            Caster.Movement.MoveDirection(m_Direction.normalized, m_SpeedMul.FloatValue);
        }
    }
}
