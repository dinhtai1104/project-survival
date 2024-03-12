using Engine;
using Engine.State.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine.State.Unit
{
    public class UnitDashState : BaseActorState
    {
        [SerializeField] private string m_Animation = "move";
        [SerializeField] protected float m_TimeDash;
        [SerializeField] protected float m_DashMulSpeed = 5;

        private Vector2 _direction;
        private float m_CurrentTimeDash = 0;

        public Vector2 Direction { get => _direction; set => _direction = value; }
        public float TimeDash { get => m_TimeDash; set => m_TimeDash = value; }
        public float DashMulSpeed { get => m_DashMulSpeed; set => m_DashMulSpeed = value; }

        public override void Enter()
        {
            base.Enter();
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
                if (Actor.Movement.ReachBound)
                {
                    ToIdleState();
                }
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
