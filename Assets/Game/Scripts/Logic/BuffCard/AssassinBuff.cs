using Game.GameActor;
using Game.GameActor.Buff;
using UnityEngine;

namespace Game.BuffCard
{
    public class AssassinBuff : AbstractBuff
    {
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        }

        private void OnBeforeHit(ActorBase attacker, ActorBase defender, DamageSource damageSource)
        {
            if (attacker == Caster && defender.GetCharacterType()!=ECharacterType.Boss)
            {
                if (attacker.GetTransform().localEulerAngles.y == defender.GetTransform().localEulerAngles.y)
                {
                    // crit
                    damageSource.IsCrit = true;
                }
            }
        }
        public override void Play()
        {
        }
    }
}