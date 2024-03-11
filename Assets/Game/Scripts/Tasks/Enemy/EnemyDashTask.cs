using Assets.Game.Scripts.Actor.States.Common;
using Assets.Game.Scripts.Actor.States.Unit;
using Engine;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks.Enemy
{
    public class EnemyDashTask : SkillTask
    {
        [SerializeField] private BindGameConfig m_DashSpeedMultiplyConfig = new("[{0}]Skill_DashSpeedMultiply", 2);
        [SerializeField] private BindGameConfig m_DashDurationConfig = new("[{0}]Skill_DashDuration", 1f);

        public override void Begin()
        {
            m_DashSpeedMultiplyConfig.SetId(Caster.gameObject.name);
            m_DashDurationConfig.SetId(Caster.gameObject.name);
            base.Begin();
            if (Caster.Fsm.HasState<UnitDashState>())
            {
                var state = Caster.Fsm.GetState<UnitDashState>();
                state.DashMulSpeed = m_DashSpeedMultiplyConfig.FloatValue;
                state.TimeDash = m_DashDurationConfig.FloatValue;

                Caster.Fsm.ChangeState<UnitDashState>();
            }
            else
            {
                Debug.Log($"====> NOT FOUND THIS STATE {typeof(UnitDashState)} <====");
            }
        }
    }
}
