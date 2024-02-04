using com.mec;
using Cysharp.Threading.Tasks;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class RangeShotOverwriteAnimation : RangeShotTask
    {
        private int number = 0;
        public override UniTask Begin()
        {
            number = 0;
            return base.Begin();
        }
        protected override void OnCompleteTracking(TrackEntry trackEntry)
        {
            if (number >= bulletNumber.IntValue)
            {
                IsCompleted = true;
            }
            else
            {
                //Caster.AnimationHandler.SetAnimation(1, "idle", true);
            }
        }
        protected override void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            if (number >= bulletNumber.IntValue)
            {
                IsCompleted = true;
            }
            else
            {
                number++;
                base.OnEventTracking(trackEntry, e);
                Timing.RunCoroutine(_WaitForAttack(), gameObject);
            }
        }

        protected override void ReleaseBullet(Transform pos)
        {
            var target = Caster.FindClosetTarget();
            if (target == null)
            {
                target = GameController.Instance.GetMainActor();
            }
            if (target == null)
            {
                IsCompleted = true;
                Timing.KillCoroutines(gameObject);
                return;
            }

            ReleaseBullet(pos, 180);
        }

        private IEnumerator<float> _WaitForAttack()
        {
            yield return Timing.WaitForSeconds(bulletDelayBtwShot.FloatValue);
            Caster.AnimationHandler.SetAnimation(1, animationSkill, false);
        }
    }
}
