using AIState;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorState
{
    public abstract class BaseCharacterState : BaseState
    {
        public override void Execute()
        {
            base.Execute();
            if (Actor.Health.HealthPercentage <= 0f)
            {
                Actor.Fsm.ChangeState<DeathState>();
            }
        }

        protected virtual void ToIdleState()
        {
            Actor.Fsm.ChangeState<IdleState>();
        }

        protected void ToMoveState()
        {
            Actor.Fsm.ChangeState<MoveState>();
        }

        protected void ToDashState()
        {
            Actor.Fsm.ChangeState<DashState>();
        }

        protected void ToDeathState()
        {
            Actor.Fsm.ChangeState<DeathState>();
        }
    }
}
