using DG.Tweening;
using Engine;
using ExtensionKit;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AIState
{
    public class ActorBornState : BaseState
    {
        [SerializeField] private string m_Animation;
        [SerializeField] private bool m_Loop;
        public override void Enter()
        {
            base.Enter();
            Actor.IsActivated = false;
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
            Actor.IsActivated = true;
        }

        private void ToIdleState()
        {
            Actor.Fsm.ChangeState<ActorIdleState>();
        }
    }
}
