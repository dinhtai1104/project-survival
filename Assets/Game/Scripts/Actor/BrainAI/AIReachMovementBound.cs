using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_ReachMovementBound.asset", menuName = SOUtility.GAME_AI + "AI_ReachMovementBound")]
public class AIReachMovementBound : BrainDecision
{
    public override bool Decide(Actor actor)
    {
        return actor.Movement.ReachBound;
    }
}
