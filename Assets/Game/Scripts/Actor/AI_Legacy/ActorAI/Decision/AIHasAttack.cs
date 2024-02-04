using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AI_HasAttack", menuName = "AI/AI_HasAttack")]
    public class AIHasAttack : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            var target = actor.FindClosetTarget();
            return target != null;
        }
    }
}