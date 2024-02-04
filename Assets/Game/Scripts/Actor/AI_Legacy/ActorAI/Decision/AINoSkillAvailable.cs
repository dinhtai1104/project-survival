using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AI_NoSkillAvailable", menuName = "AI/AI_NoSkillAvailable")]
    public class AINoSkillAvailable : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            return !actor.SkillEngine.IsBusy && !actor.SkillEngine.IsSkillAvailable;
        }
    }
}