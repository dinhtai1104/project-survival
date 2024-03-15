using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_IsSkillBusy.asset", menuName = SOUtility.GAME_AI + "AI_IsSkillBusy")]
public class AIIsSkillBusy : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        return actor.SkillCaster.IsBusy;
    }
}
