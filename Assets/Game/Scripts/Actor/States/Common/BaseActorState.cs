using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Events;
using Core;
using Engine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Common
{
    public class BaseActorState : BaseState
    {
        public override void Enter()
        {
            base.Enter();
            Architecture.Get<EventMgr>().Subscribe<DamageAfterHitEventArgs>(OnHurt);
        }
        public override void Exit()
        {
            base.Exit();
            Architecture.Get<EventMgr>().Unsubscribe<DamageAfterHitEventArgs>(OnHurt);
        }

        protected virtual void OnHurt(object sender, IEventArgs e)
        {
            var evt = e as DamageAfterHitEventArgs;
            var defender = evt.defender;
            var attacker = evt.attacker;
            if (defender != Actor) return;
            if (defender is PlayerActor) return;
            var hitResult = evt.hitResult;

            var _hurtDir = Vector3.Normalize(defender.BotPosition - attacker.BotPosition);
            var _dest = defender.BotPosition + _hurtDir * attacker.Stats.GetValue(StatKey.Knockback, 1) * 0.2f;
            defender.Movement.MoveTween(_dest, 0.3f);
            if (hitResult.Hurt)
            {
            }
        }

        public override void Execute()
        {
            base.Execute();
            if (Actor.Health.HealthPercentage <= 0f && !Actor.Health.Invincible)
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
