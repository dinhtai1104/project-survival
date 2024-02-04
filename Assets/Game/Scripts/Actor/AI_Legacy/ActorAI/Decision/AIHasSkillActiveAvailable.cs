using Game.AI;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_HasSkillActiveAvailable", menuName = "AI/AI_HasSkillActiveAvailable")]
public class AIHasSkillActiveAvailable : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        return actor.SkillEngine.IsSkillAvailable;
    }
}
