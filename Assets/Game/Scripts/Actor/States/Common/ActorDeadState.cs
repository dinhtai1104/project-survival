using Core;
using Engine;
using Events;
using Spine;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Common
{
    public class ActorDeadState : BaseActorState
    {
        [SerializeField] private string m_Animation;
        public override void Enter()
        {
            base.Enter();
            Actor.Animation.EnsurePlay(0, m_Animation, false);
            Actor.Animation.SubscribeComplete(CompleteAnimation);

            Architecture.Get<EventMgr>().Fire(this, new ActorDieEventArgs(Actor));
        }
        public override void Exit()
        {
            base.Exit();
            Actor.Animation.UnsubcribeComplete(CompleteAnimation);
        }

        private void CompleteAnimation(TrackEntry trackEntry)
        {
            if (trackEntry.TrackCompareAnimation(m_Animation))
            {
                Actor.gameObject.SetActive(false);
            }
        }
    }
}
