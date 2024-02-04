using Assets.Game.Scripts.Utilities;
using com.mec;
using Cysharp.Threading.Tasks;
using Game.Effect;
using Game.GameActor;
using Game.Pool;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_3
{
    public class Enemy9003Skill0Task : AnimationRequireSkillTask
    {
        public Transform leftEye, rightEye;
        public bool RunOnStart = false;
        public Enemy9003BulletRay bulletRayPrefab;
        public BulletSimpleDamageObject bulletPrefab;
        public ValueConfigSearch bulletSize;
        public ValueConfigSearch bulletDmg;
        public ValueConfigSearch bulletAmount;
        public ValueConfigSearch bulletVelocity;
        public ValueConfigSearch bulletDistance;

        public ValueConfigSearch bulletDelay;

        public ValueConfigSearch rotateSpeedCircle_rightEye;
        public ValueConfigSearch rotateSpeedCircle_leftEye;

        public string VFX_Name;

        private int currentCircle = 0;
        public override async UniTask Begin()
        {
            bulletSize = bulletSize.SetId(Caster.gameObject.name);
            bulletDmg = bulletDmg.SetId(Caster.gameObject.name);
            bulletAmount = bulletAmount.SetId(Caster.gameObject.name);
            bulletDistance = bulletDistance.SetId(Caster.gameObject.name);
            rotateSpeedCircle_rightEye = rotateSpeedCircle_rightEye.SetId(Caster.gameObject.name);
            rotateSpeedCircle_leftEye = rotateSpeedCircle_leftEye.SetId(Caster.gameObject.name);
            bulletVelocity = bulletVelocity.SetId(Caster.gameObject.name);
            bulletDelay = bulletDelay.SetId(Caster.gameObject.name);

            if (RunOnStart)
            {
                ReleaseBullet(leftEye);
                ReleaseBullet(rightEye);
                IsCompleted = true;
                return;
            }
            else
            {
                await base.Begin();
            }
        }

        protected override void AnimationHandler_onEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            base.AnimationHandler_onEventTracking(trackEntry, e);
            if (trackEntry.TrackCompareAnimation(animationSkill) && e.EventCompare("attack_tracking"))
            {
                ReleaseBullet(leftEye);
                ReleaseBullet(rightEye);
            }
        }

        protected virtual void SpawnVFXMuzzle(Vector3 pos)
        {
            if (!string.IsNullOrEmpty(VFX_Name))
            {
                GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
                {
                    t.GetComponent<EffectAbstract>().Active(pos);
                });
            }
        }
        private void ReleaseBullet(Transform pos)
        {
            var valueRotate = pos == leftEye ? rotateSpeedCircle_leftEye : rotateSpeedCircle_rightEye;

            var bulletRay = PoolManager.Instance.Spawn(bulletRayPrefab);
            bulletRay.GetComponent<AutoFollowObject>().SetFollow(pos);
            bulletRay.transform.position = pos.position;
            bulletRay.SetCaster(Caster);

            var rotateSpeed = new Stat(valueRotate.FloatValue);
            bulletRay.Rotate.Speed = rotateSpeed;
            Timing.RunCoroutine(_Shot());

            IEnumerator<float> _Shot()
            {
                var distance = bulletDistance.FloatValue;

                for (int i = bulletAmount.IntValue; i > 0; i--)
                {
                    SpawnVFXMuzzle(pos.position);
                    var bullet = ReleaseBulletInRay(bulletRay.transform);
                    var target = Vector3.zero + i * distance * Vector3.down;
                    bulletRay.AddBullet(target, bullet);
                    yield return Timing.WaitForSeconds(bulletDelay.FloatValue);
                }
                //while(!bulletRay.IsCompleteSpawn())
                //{
                //    yield return Timing.DeltaTime;
                //}
                bulletRay.Play();
            }
        }

        public BulletSimpleDamageObject ReleaseBulletInRay(Transform ray)
        {

            var bullet = PoolManager.Instance.Spawn(bulletPrefab, ray);
            var statSpeed = new Stat(bulletVelocity.FloatValue);

            var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
            Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
            bullet.transform.localPosition = Vector3.zero;
            bullet.SetCaster(Caster);
            bullet.Movement.Speed = statSpeed;
            bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.FloatValue);
            return bullet;
        }
    }
}
