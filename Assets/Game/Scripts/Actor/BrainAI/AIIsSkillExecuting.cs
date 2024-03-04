using System.Collections;
using System.Collections.Generic;
using Engine;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_IsSkillExecuting.asset", menuName = SOUtility.GAME_AI + "AI_IsSkillExecuting")]
public class AIIsSkillExecuting : BrainDecision
{
    [SerializeField] private int m_SkillId = -1;

    public override bool Decide(Actor actor)
    {
        if (m_SkillId != -1)
        {
            if (actor.SkillCaster.HasSkillId(m_SkillId))
            {
                return actor.SkillCaster.GetSkillById(m_SkillId).IsExecuting;
            }
        }

        return actor.SkillCaster.IsExecuting;
    }
}