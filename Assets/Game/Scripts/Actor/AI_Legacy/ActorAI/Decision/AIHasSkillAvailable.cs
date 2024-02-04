using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AI_HasSKillAvailable", menuName = "AI/AI_HasSKillAvailable")]
    public class AIHasSkillAvailable : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            if (actor.gameObject.name.Contains("0"))
            {
                Logger.Log("CHECK: "+actor.SkillEngine.IsSkillAvailable +" "+ actor.SkillEngine.IsBusy+" => "+(actor.SkillEngine.IsSkillAvailable && !actor.SkillEngine.IsBusy));
            }
            return actor.SkillEngine.IsSkillAvailable && !actor.SkillEngine.IsBusy;
        }
    }
}