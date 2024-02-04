using Game.GameActor;

namespace Game.AI
{
    public class AITouchTarget : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            var target = actor.FindClosetTarget();
            if (target != null)
            {
                return true;
            }
            return false;
        }
    }
}