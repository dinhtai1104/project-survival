using Assets.Game.Scripts.Utilities;
using com.mec;
using Cysharp.Threading.Tasks;
using ExtensionKit;
using Game.Effect;
using Game.GameActor;
using Game.Pool;
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
    public class RangeShotCircleBulletTask : AnimationRequireSkillTask
    {
        public Transform pos;
        public string VFX_Name;
        [Header("Circle Bullet")]
        public CirclePatternBulletSpawner CirclePrefab;
        public ValueConfigSearch circle_NumberBullet;
        public ValueConfigSearch circle_Velocity;
        public ValueConfigSearch circle_RotationSpeed;
        public ValueConfigSearch circle_Number;
        public ValueConfigSearch circle_Delay;


        [Header("Bullet Inside")]
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletSize;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletVelocity;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletDmg;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletReflect;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletChaseLevel;
        [BoxGroup("Range Shot")] public ValueConfigSearch bulletDelayBtwShot = new ValueConfigSearch("[{0}]BulletDelayBtwShot", "0.2");

        public UnityEvent eventShoot;

        private int current = 0;
        public override async UniTask Begin()
        {
            current = 0;
            await base.Begin();
            if (GameController.Instance.GetMainActor() == null)
            {
                IsCompleted = true;
                Skill.Stop();
                return;
            }
            bulletSize = bulletSize.SetId(Caster.gameObject.name);
            bulletVelocity = bulletVelocity.SetId(Caster.gameObject.name);
            bulletDmg = bulletDmg.SetId(Caster.gameObject.name);
            bulletReflect = bulletReflect.SetId(Caster.gameObject.name);
            circle_Number = circle_Number.SetId(Caster.gameObject.name);
            bulletChaseLevel = bulletChaseLevel.SetId(Caster.gameObject.name);
            circle_NumberBullet = circle_NumberBullet.SetId(Caster.gameObject.name);
            circle_RotationSpeed = circle_RotationSpeed.SetId(Caster.gameObject.name);
        }

        protected override void AnimationHandler_onCompleteTracking(TrackEntry trackEntry)
        {
            base.AnimationHandler_onCompleteTracking(trackEntry);
            IsCompleted = true;
        }

        protected override void AnimationHandler_onEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            base.AnimationHandler_onEventTracking(trackEntry, e);
            if (current < circle_Number.IntValue)
            {
                if (trackEntry.TrackCompareAnimation(animationSkill) && e.EventCompare("attack_tracking"))
                {
                    if (!string.IsNullOrEmpty(VFX_Name))
                    {
                        GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
                        {
                            t.GetComponent<EffectAbstract>().Active(pos.position);
                        });
                    }
                    eventShoot?.Invoke();

                    current++;
                    ReleaseCircle();
                }
            }
        }

        private void ReleaseCircle()
        {
            var angle = GetAngleToTarget(pos);
            var statSpeed = new Stat(circle_Velocity.FloatValue);
            var rotateSpeed = new Stat(circle_RotationSpeed.FloatValue);

            var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
            Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);

            var circle = PoolManager.Instance.Spawn(CirclePrefab);
            circle.transform.position = pos.position;
            circle.SetCaster(Caster);
            circle.SpawnItem(circle_NumberBullet.IntValue, 1f, OnCreate);
            circle.Movement.Speed = statSpeed;
            circle.Rotate.Speed = rotateSpeed;
            circle.transform.eulerAngles = Vector3.forward * angle;

            circle.Play();
            Timing.RunCoroutine(_WaitForAttack(circle_Delay.FloatValue));
        }

        private void OnCreate(CharacterObjectBase obj)
        {
            var bullet = obj as BulletSimpleDamageObject;
            bullet.transform.LocalScale(bulletSize.FloatValue);
            bullet.SetCaster(Caster);
            var statSpeed = new Stat(bulletVelocity.FloatValue);

            var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
            Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
            bullet.Movement.Speed = statSpeed;
            bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.SetId(Caster.gameObject.name).FloatValue);
            bullet.SetMaxHit(bulletReflect.IntValue > 1 ? bulletReflect.IntValue : 1);
            bullet.SetMaxHitToTarget(1);
            bullet.Movement.TrackTarget(bulletChaseLevel.FloatValue, GameController.Instance.GetMainActor().transform);
            bullet.Movement.SetMove(bullet.transform.up);
            bullet.Play();
        }

        private IEnumerator<float> _WaitForAttack(float floatValue)
        {
            yield return Timing.WaitForSeconds(floatValue);
            Caster.AnimationHandler.SetAnimation(trackAnim, animationSkill, false);
        }
    }
}
