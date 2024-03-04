using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Assets.Game.Scripts.Actor.States.Common
{
    public class BaseActorState : BaseState
    {
        public override void Execute()
        {
            base.Execute();
            if (Actor.Health.HealthPercentage <= 0f)
            {
                Actor.Fsm.ChangeState<ActorDeadState>();
            }
        }

        protected virtual void ToIdleState()
        {
            Actor.Fsm.ChangeState<ActorIdleState>();
        }

        protected void ToMoveState()
        {
            Actor.Fsm.ChangeState<ActorMoveState>();
        }

        protected void ToDashState()
        {
            Actor.Fsm.ChangeState<ActorDashState>();
        }

        protected void ToDeathState()
        {
            Actor.Fsm.ChangeState<ActorDeadState>();
        }
    }
}
