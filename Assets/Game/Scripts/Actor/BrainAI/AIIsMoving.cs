using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_IsMoving.asset", menuName = SOUtility.GAME_AI + "AI_IsMoving")]
public class AIIsMoving : BrainDecision
{
    public override bool Decide(Actor actor)
    {
        return actor.Movement.IsMoving;
    }
}