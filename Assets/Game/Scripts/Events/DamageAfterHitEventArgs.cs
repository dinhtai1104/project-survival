using Core;
using Engine;
using UnityEngine;

namespace Assets.Game.Scripts.Events
{
    public class DamageAfterHitEventArgs : BaseEventArgs<DamageAfterHitEventArgs>
    {
        public Engine.ActorBase attacker;
        public Engine.ActorBase defender;
        public HitResult hitResult;
        public DamageSource damageSource;

        public DamageAfterHitEventArgs(Engine.ActorBase attack, Engine.ActorBase defense, DamageSource source, HitResult hitResult)
        {
            this.damageSource = source;
            this.attacker = attack;
            this.defender = defense;
            this.hitResult = hitResult;

            Debug.Log("Hit: " + hitResult.Damage);
        }
    }
}
