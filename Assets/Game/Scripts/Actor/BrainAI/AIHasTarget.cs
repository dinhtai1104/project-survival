using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_HasTarget.asset", menuName = SOUtility.GAME_AI + "AI_HasTarget")]
public class AIHasTarget : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        if (actor.TargetFinder == null) return false;
        return actor.TargetFinder.CurrentTarget != null;
    }
}