using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AI_EnterCastSkill", menuName = "AI/AI_EnterCastSkill")]
    public class AIEnterCastSkill : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            return actor.SkillEngine.CastSkillRandom();
        }
    }
}