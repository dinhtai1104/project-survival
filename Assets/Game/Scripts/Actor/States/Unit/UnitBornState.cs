using DG.Tweening;
using Engine;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Unit
{
    public class UnitBornState : BaseState
    {
        [SerializeField] private string m_Animation;
        [SerializeField] private bool m_Loop;
        public override void Enter()
        {
            base.Enter();
            Actor.Health.Invincible = true;
            if (!m_Animation.IsNullOrEmpty())
            {
                Actor.Animation.SubscribeComplete(OnAnimationComplete);
                Actor.Animation.Play(0, m_Animation, m_Loop, true);
                return;
            }
            Actor.Animation.Play(0, "idle", true);

            Actor.Trans.DOScaleY(1, 0.5f).From(0).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    Actor.Fsm.ChangeState<UnitIdleState>();
                });
        }

        private void OnAnimationComplete(Spine.TrackEntry trackEntry)
        {
            if (!trackEntry.Animation.Name.Equals(m_Animation)) return;
            if (Actor != null)
            {
                Actor.Fsm.ChangeState<UnitIdleState>();
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
