using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_8_3
{
    public class SummonFireRayTask : AnimationRequireSkillTask
    {
        public CharacterDamageObject fireRayPrefab;
        private CharacterDamageObject fireRay;

        public ValueConfigSearch fireRayDuration;
        public ValueConfigSearch fireDmg_Status;
        public ValueConfigSearch fireDmgInterval_Status;
        public ValueConfigSearch fireDuration_Status;

        public LayerMask groundMask;
        public async override UniTask Begin()
        {
            if (fireRay != null)
            {
                IsCompleted = true;
                return;
            }
            await base.Begin();
        }
        protected override void AnimationHandler_onEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            base.AnimationHandler_onEventTracking(trackEntry, e);
            if (trackEntry.TrackCompareAnimation(animationSkill) && e.EventCompare("attack_tracking"))
            {
                SpawnFireRay();
            }
        }

        private void SpawnFireRay()
        {
            fireRay = PoolManager.Instance.Spawn(fireRayPrefab);
            var groundPos = GameUtility.GameUtility.GetPositionImpactRaycast(Caster.GetMidPos(), Vector3.down, 1000, groundMask);
            groundPos.x = 0;
            fireRay.transform.position = groundPos;
            fireRay.SetCaster(Caster);
            fireRay.GetComponent<AutoDestroyObject>().SetDuration(fireRayDuration.FloatValue);
            fireRay.GetComponent<AutoDestroyObject>().onComplete += () =>
            {
                fireRay = null;
            };
            fireRay._hit.SetMaxHit(-1);
            fireRay._hit.SetIsFullTimeHit(true);
            fireRay.Play();
            fireRay._hit.onTrigger = null;
            fireRay._hit.onTrigger += OnTrigger;

        }

        private async void OnTrigger(Collider2D collider, ITarget target)
        {
            if (target == null) return;
            if ((ActorBase)target == Caster) return;
            if (((ActorBase)target).GetCharacterType() != ECharacterType.Player) return;

            var flame = await ((ActorBase)target).StatusEngine.AddStatus(Caster, EStatus.Flame, this);
            if (flame != null)
            {
                flame.SetDuration(fireDuration_Status.FloatValue);
                flame.SetDmgMul(fireDmg_Status.FloatValue);
                flame.SetCooldown(fireDmgInterval_Status.FloatValue);
            }
        }
    }
}
