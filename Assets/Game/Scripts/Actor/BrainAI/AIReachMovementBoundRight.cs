using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_ReachMovementBoundRight.asset", menuName = SOUtility.GAME_AI + "AI_ReachMovementBoundRight")]
public class AIReachMovementBoundRight : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        return actor.Movement.ReachBoundRight;
    }
}
