using com.mec;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_10_3
{
    public class Enemy10003Skill2Task : RangeThrowBulletTask
    {
        public ValueConfigSearch offsetPositionX;
        public override UniTask Begin()
        {
            offsetPositionX.SetId(Caster.gameObject.name);
            return base.Begin();
        }

        protected override void ReleaseBullet(Transform pos)
        {
            var target = Caster.FindClosetTarget();
            if (target == null)
            {
                target = GameController.Instance.GetMainActor();
            }
            if (target == null) return;
            var angle = GameUtility.GameUtility.GetAngle(Caster, target);
            Timing.RunCoroutine(_Shot(pos), gameObject);
        }

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
                    ReleaseBulletOffset(pos, target.GetMidPos() + Vector3.right * (i - 1) * offsetPositionX.FloatValue);
                    yield return Timing.WaitForSeconds(bulletDelayBtwShot.FloatValue);
                }
            }
            if (RunOnStart)
            {
                IsCompleted = true;
            }
        }
        protected void ReleaseBulletOffset(Transform pos, Vector3 target)
        {
            var bullet = PoolManager.Instance.Spawn(bulletPrefab);
            bullet.transform.position = pos.position;
            bullet.SetCaster(Caster);
            bullet.SetMaxHit(1);
            bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.SetId(Caster.gameObject.name).FloatValue);
            bullet.transform.localScale = Vector3.one * bulletSize.SetId(Caster.gameObject.name).FloatValue;

            var dir = GameUtility.GameUtility.CalcBallisticVelocityVector(pos.position, target, 45, bulletVelocity.SetId(Caster.gameObject.name).FloatValue);
            bullet.Movement.Move(new Stat(0), dir);
            bullet.Play();
        }
    }
}
