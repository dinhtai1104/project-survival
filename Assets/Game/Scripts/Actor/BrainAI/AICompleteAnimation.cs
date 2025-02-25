using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_CompleteAnimation.asset", menuName = SOUtility.GAME_AI + "AI_CompleteAnimation")]
public class AICompleteAnimation : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        return actor.Animation.IsCurrentAnimationComplete;
    }
}