using Engine.State.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.State.Unit
{
    public class UnitRandomMoveState : BaseActorState
    {
        [SerializeField] private BindGameConfig m_TimeRandomMoveConfig = new("[{0}]RandomMove_TimeMove", 2);
        private float time = 0;
        private Vector2 m_Direction;


        public override void Enter()
        {
            m_TimeRandomMoveConfig.SetId(Actor.gameObject.name);
            base.Enter();
            time = 0;
            RandomDirection();
        }
        public override void Execute()
        {
            base.Execute();
            time += Time.deltaTime;
            if (time > m_TimeRandomMoveConfig.FloatValue)
            {
                time = 0;
                RandomDirection();
                if (Actor.Movement.ReachBound)
                {
                    RandomDirection();
                }
            }
            Actor.Movement.MoveDirection(m_Direction);
            if (Actor.Movement.ReachBound)
            {
                time = 0;
                RandomDirection();
            }
        }

        private void RandomDirection()
        {
            m_Direction = Random.insideUnitCircle;
        }
    }
}
