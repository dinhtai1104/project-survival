using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_SkillCasting.asset", menuName = SOUtility.GAME_AI + "AI_SkillCasting")]
public class AISkillCasting : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        return actor.SkillCaster.CastRandomAvailableSkill();
    }
}