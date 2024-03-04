using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_Dead.asset", menuName = SOUtility.GAME_AI + "AI_Dead")]
public class AIDead : BrainDecision
{
    public override bool Decide(Actor actor)
    {
        if (actor.IsDead) return true;
        actor.IsDead = (actor.Health.CurrentHealth <= 0f);
        return actor.IsDead;
    }
}