using com.mec;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_3
{
    public class Enemy9003Skill2Task : RangeShotTask
    {
        public ValueConfigSearch speedPhase2Mul;
        public ValueConfigSearch timeBeforeDashToTarget;
        protected override BulletSimpleDamageObject ReleaseBullet(Transform pos, float angle)
        {
            var bullet = base.ReleaseBullet(pos, 90);
            Timing.RunCoroutine(_AutoTarget(bullet, angle));
            return bullet;
        }

        private IEnumerator<float> _AutoTarget(BulletSimpleDamageObject bullet, float angle)
        {
            yield return Timing.WaitForSeconds(timeBeforeDashToTarget.FloatValue * 0.9f);
            angle = GameUtility.GameUtility.GetAngle(bullet.transform.position, GetPosTarget());
            bullet.Movement.SetDirection(GameUtility.GameUtility.ConvertDir(angle));
            bullet.Movement.Speed.AddModifier(new StatModifier(EStatMod.FlatMul, speedPhase2Mul.FloatValue));
        }
    }
}
