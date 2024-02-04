using com.mec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_3
{
    public class Enemy9003Skill1Task : RangeShotTask
    {
        public ValueConfigSearch lowerAngle;
        public ValueConfigSearch upperAngle;

        protected override IEnumerator<float> _Shot(Transform pos)
        {
            var target = Caster.FindClosetTarget();
            if (target == null)
            {
                target = GameController.Instance.GetMainActor();
            }
            if (target != null)
            {
                for (int i = 0; i < (bulletNumber.IntValue == 0 ? 1 : bulletNumber.IntValue); i++)
                {
                    var angle = GameUtility.GameUtility.GetRandomAngle(upperAngle.FloatValue, lowerAngle.FloatValue);
                    ReleaseBullet(pos, angle);
                    yield return Timing.WaitForSeconds(bulletDelayBtwShot.FloatValue);
                }
            }

            if (RunOnStart)
            {
                IsCompleted = true;
            }
        }
    }
}
