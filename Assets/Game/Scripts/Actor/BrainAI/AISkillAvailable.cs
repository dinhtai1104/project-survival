using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_SkillAvailable.asset", menuName = SOUtility.GAME_AI + "AI_SkillAvailable")]
public class AISkillAvailable : BrainDecision
{
    public override bool Decide(Actor actor)
    {
        return actor.SkillCaster.HasAvailableSkill;
    }
}
