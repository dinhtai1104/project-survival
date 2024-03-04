using Engine;
using MoreMountains.Tools;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Common
{
    public class ActorBornState : BaseActorState
    {
        [SerializeField] private string m_Animation;
        [SerializeField] private bool m_Loop;
        public override void Enter()
        {
            base.Enter();
            Actor.Animation.SubscribeComplete(OnAnimationComplete);
            Actor.Animation.Play(0, m_Animation, m_Loop, true);
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            if (!trackEntry.Animation.Name.Equals(m_Animation)) return;
            if (Actor != null)
            {
                Actor.Fsm.ChangeState<ActorIdleState>();
            }
        }

        public override void Exit()
        {
            Actor.Animation.UnsubcribeComplete(OnAnimationComplete);
            base.Exit();
        }
    }
}
