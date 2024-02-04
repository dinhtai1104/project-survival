using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AI_Dead", menuName = "AI/AI_Dead_Decision")]
    public class AIDead : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            return actor.PropertyHandler.GetProperty(EActorProperty.Dead, 0) == 1;
        }
    }
}