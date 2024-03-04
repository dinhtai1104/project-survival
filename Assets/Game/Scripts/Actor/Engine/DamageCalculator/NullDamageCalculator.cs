using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullDamageCalculator : IDamageCalculator
    {
        public Actor Owner { get; private set; }

        public void Init(Actor actor)
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

        public HitResult CalculateDamage(Actor defender, Actor attacker, DamageSource source)
        {
            return HitResult.FailedResult;
        }
    }
}