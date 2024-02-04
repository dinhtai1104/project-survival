using Game.GameActor;
using UnityEngine;

namespace Game.GameActor.Buff
{
    public class PoisonAttackBuff : AbstractBuff
    {
        public ValueConfigSearch poisonAttack_Duration = new ValueConfigSearch("Buff_PoisonAttack_Duration");
        public ValueConfigSearch poisonAttack_Tickrate = new ValueConfigSearch("Buff_PoisonAttack_Tickrate");
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEventPoison);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEventPoison);
        }

        private async void OnAttackEventPoison(ActorBase attacker, ActorBase defender)
        {
            if (Caster == attacker)
            {
                var status = (await defender.StatusEngine.AddStatus(attacker, EStatus.Poison, this));
                if (status != null)
                {
                    status.Init(attacker, defender);
                    status.SetDuration(poisonAttack_Duration.FloatValue);
                    ((PoisonStatus)status).SetDmgMul(GetValue(StatKey.Dmg));
                    ((PoisonStatus)status).SetCooldown(poisonAttack_Tickrate.FloatValue);
                }
            }
        }

        public override void Play()
        {
            
        }
    }
}