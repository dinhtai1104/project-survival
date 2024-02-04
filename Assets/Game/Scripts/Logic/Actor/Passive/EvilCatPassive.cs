using Game.GameActor.Buff;
using System;

namespace Game.GameActor.Passive
{
    public class EvilCatPassive : AbstractBuff
    {
        public override void Play()
        {
        }
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
        }

        private void OnKill(ActorBase attacker, ActorBase defender)
        {
            if (attacker == Caster)
            {
                var heal = GetValue(StatKey.Hp) * Caster.HealthHandler.GetMaxHP();
                Caster.Heal((int)heal,true);
            }
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
        }
    }
}