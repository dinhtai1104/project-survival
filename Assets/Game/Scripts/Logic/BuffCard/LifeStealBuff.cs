using Game.GameActor;
using Game.GameActor.Buff;
using Game.Skill;
using System;
using UnityEngine;

namespace Game.BuffCard
{
    public class LifeStealBuff : AbstractBuff
    {
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
        }

        private void OnKill(ActorBase attacker, ActorBase defender)
        {
            if (attacker == Caster && defender != Caster)
            {
                var hpPercentHeal = GetValue(StatKey.Hp);
                var value = Caster.HealthHandler.GetMaxHP() * (hpPercentHeal + attacker.GetStatValue(StatKey.LifestealMul));
                Debug.Log("[Buff] Life steal added after caster: " + value);

                Caster.Heal((int)value, true);
            }
        }

        public override void Play()
        {
        }
    }
}