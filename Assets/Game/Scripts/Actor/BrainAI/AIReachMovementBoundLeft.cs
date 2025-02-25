using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_ReachMovementBoundLeft.asset", menuName = SOUtility.GAME_AI + "AI_ReachMovementBoundLeft")]
public class AIReachMovementBoundLeft : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        return actor.Movement.ReachBoundLeft;
    }
}
