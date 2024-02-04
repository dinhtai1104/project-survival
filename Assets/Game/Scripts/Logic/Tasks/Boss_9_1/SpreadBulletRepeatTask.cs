using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_1
{
    public class SpreadBulletRepeatTask : SkillTask
    {
        public ValueConfigSearch delayBtwShot;
        private float time;

        [BoxGroup("Range Shot")] public BulletSimpleDamageObject bulletPrefab;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletSize;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletVelocity;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletDmg;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletReflect;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletNumber;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletChaseLevel;

        public UnityEvent eventShoot;

        public override void Run()
        {
            if (!Skill.IsExecuting)
            {
                return;
            }
            time += Time.deltaTime;
            if (time > delayBtwShot.FloatValue)
            {
                time = 0;
                eventShoot?.Invoke();
                SpreadShot();
            }
        }

        private void SpreadShot()
        {
            ReleaseBullet(Caster.GetMidTransform());
        }

        protected void ReleaseBullet(Transform pos)
        {
            //base.ReleaseBullet();
            float angle = 0;
            float angleIncre = 360f / bulletNumber.IntValue;
            for (int i = 0; i < bulletNumber.IntValue; i++)
            {
                ReleaseBullet(pos, angle);
                angle += angleIncre;
            }
        }
        protected virtual BulletSimpleDamageObject ReleaseBullet(Transform pos, float angle)
        {
            
            var bullet = PoolManager.Instance.Spawn(bulletPrefab);
            bullet.transform.position = pos.position;

            bullet.transform.eulerAngles = Vector3.forward * angle;
            bullet.transform.localScale = Vector3.one * bulletSize.FloatValue;

            var statSpeed = new Stat(bulletVelocity.FloatValue);

            var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
            Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
            bullet.Movement.Speed = statSpeed;
            bullet.SetCaster(Caster);
            bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.SetId(Caster.gameObject.name).FloatValue);
            bullet.SetMaxHit(bulletReflect.IntValue > 1 ? bulletReflect.IntValue : 1);
            bullet.SetMaxHitToTarget(1);
            bullet.Play();
            bullet.Movement.TrackTarget(bulletChaseLevel.FloatValue, GameController.Instance.GetMainActor().transform);
            return bullet;
        }
    }
}
