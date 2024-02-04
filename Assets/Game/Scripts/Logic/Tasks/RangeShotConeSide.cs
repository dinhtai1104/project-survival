using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using ExtensionKit;
using Game.Effect;
using Game.GameActor;
using Game.Handler;
using Sirenix.OdinInspector;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class RangeShotConeSide : AnimationRequireSkillTask
    {
        public string VFX_Name = "";

        [BoxGroup("Range Shot")] public BulletSimpleDamageObject bulletPrefab;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletSize;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletVelocity;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletDmg;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletReflect;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletNumber;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletChaseLevel;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletDelayBtwShot = new ValueConfigSearch("[{0}]BulletDelayBtwShot", "0.2");
        [BoxGroup("Range Shot")] public ValueConfigSearch angleBtwBulletCone;


        [BoxGroup("Range Shot")] public bool bulletIsForcusTarget = true;
        [BoxGroup("Range Shot")] public Transform pos;
        public UnityEvent eventShoot;

        public override async UniTask Begin()
        {
            if (GameController.Instance.GetMainActor() == null)
            {
                Skill.Stop();
                IsCompleted = true;
                return;
            }
            await base.Begin();

            bulletSize = bulletSize.SetId(Caster.gameObject.name);
            bulletVelocity = bulletVelocity.SetId(Caster.gameObject.name);
            bulletDmg = bulletDmg.SetId(Caster.gameObject.name);
            bulletReflect = bulletReflect.SetId(Caster.gameObject.name);
            bulletNumber = bulletNumber.SetId(Caster.gameObject.name);
            bulletChaseLevel = bulletChaseLevel.SetId(Caster.gameObject.name);
            angleBtwBulletCone = angleBtwBulletCone.SetId(Caster.gameObject.name);
        }


        protected override void AnimationHandler_onEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            base.AnimationHandler_onEventTracking(trackEntry, e);
            if (trackEntry.TrackCompareAnimation(animationSkill) && e.EventCompare("attack_tracking"))
            {
                OnEventAttrach();
            }
        }

        protected virtual void OnEventAttrach()
        {
            var angleToTarget = GetAngleToTarget(pos);

            SpawnEffect(pos);
            SpawnCone(pos, angleToTarget);
        }

        protected void SpawnEffect(Transform pos)
        {
            if (VFX_Name.IsNotNullAndEmpty())
            {
                EffectHandler.GetEffect(VFX_Name, (ef) =>
                {
                    ef.GetComponent<EffectAbstract>().Active(pos);
                });
            }
        }

        protected virtual void SpawnCone(Transform pos, float angleMid)
        {
            float currentAngle = angleMid;
            float offsetAngle = angleBtwBulletCone.FloatValue;

            ReleaseBullet(pos, currentAngle);

            for (int i = 0; i < bulletNumber.IntValue / 2; i++)
            {
                ReleaseBullet(pos, currentAngle + offsetAngle);
                ReleaseBullet(pos, currentAngle - offsetAngle);
                offsetAngle += angleBtwBulletCone.FloatValue;
            }
        }

        public BulletSimpleDamageObject ReleaseBullet(Transform pos, float angle)
        {
            var bullet = PoolManager.Instance.Spawn(bulletPrefab);
            bullet.transform.position = pos.position;

            if (bulletIsForcusTarget)
            {
                bullet.transform.eulerAngles = Vector3.forward * angle;
            }
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
