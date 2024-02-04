using Game.GameActor;
using UnityEngine;

namespace Game.GameActor.Buff
{
    public class FlameAttackBuff : AbstractBuff
    {
        public ValueConfigSearch flameAttack_Duration = new ValueConfigSearch("Buff_FlameAttack_Duration");
        public ValueConfigSearch flameAttack_Tickrate = new ValueConfigSearch("Buff_FlameAttack_Tickrate");
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
            if (attacker != Caster) return;
            var status = (await defender.StatusEngine.AddStatus(attacker, EStatus.Flame, this));
            if (status != null)
            {
                status.Init(attacker, defender);
                status.SetDuration(flameAttack_Duration.FloatValue);
                ((FlameStatus)status).SetDmgMul(GetValue(StatKey.Dmg));
                ((FlameStatus)status).SetCooldown(flameAttack_Tickrate.FloatValue);
            }
        }

        public override void Play()
        {

        }
    }
}