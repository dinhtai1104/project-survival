using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullDamageCalculator : IDamageCalculator
    {
        public ActorBase Owner { get; private set; }

        public void Init(ActorBase actor)
        {
            Owner = actor;
        }

        public void SetImmuneDamageType(EDamageType type, bool immune)
        {
        }

        public void AddAttackerTemporaryModifier(string modName, StatModifier mod)
        {
        }

        public void AddDefenderTemporaryModifier(string modName, StatModifier mod)
        {
        }

        public HitResult CalculateDamage(ActorBase defender, ActorBase attacker, DamageSource source)
        {
            return HitResult.FailedResult;
        }
    }
}