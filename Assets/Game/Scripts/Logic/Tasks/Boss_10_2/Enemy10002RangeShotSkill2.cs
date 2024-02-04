using com.mec;
using Cysharp.Threading.Tasks;
using ExtensionKit;
using Game.Effect;
using Game.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_10_2
{
    public class Enemy10002RangeShotSkill2 : RangeShotTask
    {
        private float YPosition;
        public ValueConfigSearch spaceBullet;
        private int index = 0;
        private List<BulletSimpleDamageObject> listBullet = new List<BulletSimpleDamageObject>();
        private void Awake()
        {
            YPosition = pos.position.y;
        }
        public override UniTask Begin()
        {
            listBullet.Clear();
            index = 0;
            spaceBullet.SetId(Caster.gameObject.name);
            return base.Begin();
        }
        public override UniTask End()
        {
            pos.PositionY(YPosition);
            return base.End();
        }
        public override void OnStop()
        {
            pos.PositionY(YPosition);
            base.OnStop();
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
                    var angle = GameUtility.GameUtility.GetAngle(pos, target.GetMidTransform());
                    pos.PositionY(YPosition - index * spaceBullet.FloatValue);
                    listBullet.Add(ReleaseBullet(pos, 180));
                    index++;
                    yield return Timing.WaitForSeconds(bulletDelayBtwShot.FloatValue);
                }
            }
            index = 1;
            foreach (var bullet in listBullet)
            {
                index++;
                Timing.RunCoroutine(_DelayBullet(bullet, index));
            }

            if (RunOnStart)
            {
                IsCompleted = true;
            }
        }

        private IEnumerator<float> _DelayBullet(BulletSimpleDamageObject bullet, int index)
        {
            yield return Timing.WaitForSeconds(index * bulletDelayRelease.FloatValue);
            bullet.Play();
        }

        protected override void DelayRelease(BulletSimpleDamageObject bullet, float delay)
        {
            //base.DelayRelease(bullet, bulletDelayBtwShot.FloatValue * bulletNumber.IntValue + delay * index + 1);
        }
    }
}
