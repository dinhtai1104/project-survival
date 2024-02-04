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
using UnityEngine.Events;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_8_1
{
    public class SummonThunderStormTask : AnimationRequireSkillTask
    {
        [SerializeField] private CharacterObjectBase thunderStormPrefab;
        [SerializeField] ValueConfigSearch thunderStormDuration;
        [SerializeField] ValueConfigSearch affectSpeedMod;
        [SerializeField] ValueConfigSearch affectDmgIncrease;

        private CharacterObjectBase thunderStormObj;
        private bool isThunderStormIsAvailable;
        private StatModifier speedAffectToPlayer;

        public UnityEvent EventTracking;

        private void Awake()
        {
            speedAffectToPlayer = new StatModifier(EStatMod.PercentAdd, affectSpeedMod.FloatValue);
        }

        protected override void AnimationHandler_onEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            base.AnimationHandler_onEventTracking(trackEntry, e);
            if (trackEntry.TrackCompareAnimation(animationSkill) && e.EventCompare("attack_tracking"))
            {
                EventTracking?.Invoke();
                thunderStormObj = PoolManager.Instance.Spawn(thunderStormPrefab);
                thunderStormObj.transform.position = Vector3.zero;
                thunderStormObj.SetCaster(Caster);
                thunderStormObj.GetComponent<AutoDestroyObject>().SetDuration(thunderStormDuration.FloatValue);
                thunderStormObj.GetComponent<AutoDestroyObject>().onComplete += OnBeforeDestroyThunderStorm;
                thunderStormObj.Play();
                isThunderStormIsAvailable = true;
                var player = GameController.Instance.GetMainActor();
                if (player != null)
                {
                    player.Stats.AddModifier(StatKey.SpeedMove, speedAffectToPlayer, Caster);
                }
            }
        }

        private void OnBeforeDestroyThunderStorm()
        {
            isThunderStormIsAvailable = false;
            var player = GameController.Instance.GetMainActor();
            if (player != null)
            {
                player.Stats.RemoveModifier(StatKey.SpeedMove, speedAffectToPlayer);
            }
        }

        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        }

        private void OnBeforeHit(ActorBase atk, ActorBase def, DamageSource source)
        {
            if (atk != Caster) return;
            if (isThunderStormIsAvailable)
            {
                source._damage.AddModifier(new StatModifier(EStatMod.PercentAdd, affectDmgIncrease.FloatValue));
            }
        }
    }
}
