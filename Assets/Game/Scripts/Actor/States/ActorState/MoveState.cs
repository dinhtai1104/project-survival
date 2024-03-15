using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorState
{
    public class MoveState : BaseCharacterState
    {
        [SerializeField] private string m_Animation = "run/normal";
        public override void Enter()
        {
            base.Enter();
            var inputHandler = Actor.Input;
            inputHandler.SubscribeControl(EControlCode.Dash, ToDashState);
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
            inputHandler.UnsubscribeControl(EControlCode.Dash, ToDashState);
        }
    }
}