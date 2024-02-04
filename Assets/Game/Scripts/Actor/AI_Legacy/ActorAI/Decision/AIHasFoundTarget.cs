using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AI_HasFoundTarget", menuName = "AI/AI_HasFoundTarget")]
    public class AIHasFoundTarget : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            return actor.FindClosetTarget() != null;
        }
    }
}