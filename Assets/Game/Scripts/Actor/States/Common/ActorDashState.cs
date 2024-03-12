using Engine;
using UnityEngine;
namespace Engine.State.Common
{
    public class ActorDashState : ActorSkillState
    {
        [SerializeField] private string m_Animation = "run/fight";
        [SerializeField] protected float m_TimeDash;
        [SerializeField] protected float m_DashMulSpeed = 5;

        private Vector2 _direction;
        private float m_CurrentTimeDash = 0;

        public Vector2 Direction { get => _direction; set => _direction = value; }

        public override void Enter()
        {
            base.Enter();
            _direction = Actor.Movement.CurrentDirection;
            m_CurrentTimeDash = 0;
        }


        public override void Execute()
        {
            base.Execute();
            m_CurrentTimeDash += Time.deltaTime;
            if (m_CurrentTimeDash < m_TimeDash)
            {
                Actor.Animation.EnsurePlay(0, m_Animation, true);
                Actor.Movement.MoveDirection(_direction, m_DashMulSpeed);
            }
            else
            {
                ToIdleState();
            }
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
