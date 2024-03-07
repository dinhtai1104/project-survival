using DG.Tweening;
using Engine;
using ExtensionKit;
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
            Actor.Animation.Clear();
            Actor.Health.Invincible = true;
            if (!m_Animation.IsNullOrEmpty())
            {
                Actor.Animation.SubscribeComplete(OnAnimationComplete);
                Actor.Animation.EnsurePlay(0, m_Animation, m_Loop, true);
                return;
            }

            Actor.Animation.EnsurePlay(0, "idle", m_Loop, true);
            Actor.Trans.DOScaleY(1, 0.5f).From(0).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    ToIdleState();
                });
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            if (!trackEntry.Animation.Name.Equals(m_Animation)) return;
            if (Actor != null)
            {
                ToIdleState();
            }
        }

        public override void Exit()
        {
            if (!m_Animation.IsNullOrEmpty())
            {
                Actor.Animation.UnsubcribeComplete(OnAnimationComplete);
            }
            base.Exit();
            Actor.Health.Invincible = false;
        }
    }
}
