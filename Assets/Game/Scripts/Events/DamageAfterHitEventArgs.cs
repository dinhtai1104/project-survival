using Core;
using Engine;
using UnityEngine;

namespace Assets.Game.Scripts.Events
{
    public class DamageAfterHitEventArgs : BaseEventArgs<DamageAfterHitEventArgs>
    {
        public Engine.Actor attacker;
        public Engine.Actor defender;
        public HitResult hitResult;
        public DamageSource damageSource;

        public DamageAfterHitEventArgs(Engine.Actor attack, Engine.Actor defense, DamageSource source, HitResult hitResult)
        {
            this.damageSource = source;
            this.attacker = attack;
            this.defender = defense;
            this.hitResult = hitResult;

            Debug.Log("Hit: " + hitResult.Damage);
        }
    }
}
