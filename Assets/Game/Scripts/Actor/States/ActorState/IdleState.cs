using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorState
{
    public class IdleState : BaseCharacterState
    {
        [SerializeField] private string m_Animation;
        public override void Enter()
        {
            base.Enter();
            Actor.Input.SubscribeControl(EControlCode.Move, ToMoveState);
            Actor.Input.SubscribeControl(EControlCode.Dash, ToDashState);
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
            Actor.Input.UnsubscribeControl(EControlCode.Move, ToMoveState);
            Actor.Input.UnsubscribeControl(EControlCode.Dash, ToDashState);
        }
    }
}
