using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Common
{
    public class ActorMoveState : BaseActorState
    {
        [SerializeField] private string m_Animation = "run/normal";
        public override void Enter()
        {
            base.Enter();
            var inputHandler = Actor.Input;
            inputHandler.SubscribeControl(ControlCode.Dash, ToDashState);
        }

        public override void Execute()
        {
            base.Execute();

            Actor.Movement.MoveDirection(Actor.Input.JoystickDirection);
            Actor.Animation.EnsurePlay(0, m_Animation, true);
            if (!Actor.Input.IsUsingJoystick)
            {
                ToIdleState();
            }
        }

        public override void Exit()
        {
            base.Exit();
            var inputHandler = Actor.Input;
            inputHandler.UnsubscribeControl(ControlCode.Dash, ToDashState);
        }
    }
}
