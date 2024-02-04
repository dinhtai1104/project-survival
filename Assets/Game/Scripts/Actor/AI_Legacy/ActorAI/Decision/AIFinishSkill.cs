using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AIFinishSkill", menuName = "AI/AIFinishSkill")]
    public class AIFinishSkill : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            return !actor.SkillEngine.IsBusy;
        }
    }
}