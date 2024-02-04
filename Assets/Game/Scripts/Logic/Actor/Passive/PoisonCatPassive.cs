using Game.GameActor.Buff;
using System;
using UnityEngine;

namespace Game.GameActor.Passive
{
    public class PoisonCatPassive : AbstractBuff
    {
        public ValueConfigSearch poisonCatAttack_Duration = new ValueConfigSearch("Buff_PoisonCatAttack_Duration");
        public ValueConfigSearch poisonCatAttack_Tickrate = new ValueConfigSearch("Buff_PoisonCatAttack_Tickrate");
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
        }

        private async void OnAttackEvent(ActorBase attacker, ActorBase defender)
        {
            if (attacker == Caster)
            {
                var poisonBuff = await defender.StatusEngine.AddStatus(attacker, EStatus.Poison, this);

                if (poisonBuff != null)
                {
                    poisonBuff.Init(attacker, defender);
                    poisonBuff.SetDuration(poisonCatAttack_Duration.FloatValue);
                    ((PoisonStatus)poisonBuff).SetDmgMul(GetValue(StatKey.Dmg));
                    ((PoisonStatus)poisonBuff).SetCooldown(poisonCatAttack_Tickrate.FloatValue);
                }
            }
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
        }
        public override void Play()
        {
            
        }
    }
}