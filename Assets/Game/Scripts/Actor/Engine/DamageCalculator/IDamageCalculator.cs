using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface IDamageCalculator
    {
        Actor Owner { get; }
        void Init(Actor actor);
        void SetImmuneDamageType(EDamageType type, bool immune);
        void AddAttackerTemporaryModifier(string modName, StatModifier mod);
        void AddDefenderTemporaryModifier(string modName, StatModifier mod);
        HitResult CalculateDamage(Actor defender, Actor attacker, DamageSource source);
    }
}