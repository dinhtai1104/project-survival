using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Common
{
    public class ActorIdleState : BaseActorState
    {
        [SerializeField] private string m_Animation;
        public override void Enter()
        {
            base.Enter();
            Actor.Input.SubscribeControl(ControlCode.Move, ToMoveState);
            Actor.Input.SubscribeControl(ControlCode.Dash, ToDashState);
            Actor.Animation.EnsurePlay(0, m_Animation, true);
        }
        public override void Execute()
        {
            base.Execute();
            if (Actor.Input.IsUsingJoystick)
            {
                ToMoveState();
            }
        }

        public override void Exit()
        {
            base.Exit();
            Actor.Input.UnsubscribeControl(ControlCode.Move, ToMoveState);
            Actor.Input.UnsubscribeControl(ControlCode.Dash, ToDashState);
        }
    }
}
