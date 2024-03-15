using AIState;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AIRandomMovementEnd.asset", menuName = SOUtility.GAME_AI + "AIRandomMovementEnd")]
public class AIRandomMovementEnd : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        if (actor.Fsm.HasState<ActorRandomMovementState>())
        {
            return actor.Fsm.GetState<ActorRandomMovementState>().IsMovingDurationEnd;
        }
        return false;
    }
}
