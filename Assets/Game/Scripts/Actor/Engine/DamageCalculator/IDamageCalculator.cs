using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface IDamageCalculator
    {
        ActorBase Owner { get; }
        void Init(ActorBase actor);
        void SetImmuneDamageType(EDamageType type, bool immune);
        void AddAttackerTemporaryModifier(string modName, StatModifier mod);
        void AddDefenderTemporaryModifier(string modName, StatModifier mod);
        HitResult CalculateDamage(ActorBase defender, ActorBase attacker, DamageSource source);
    }
}