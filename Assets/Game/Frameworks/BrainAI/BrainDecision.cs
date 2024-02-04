using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    public abstract class BrainDecision : ScriptableObject
    {
        public abstract bool Decide(ActorBase actor);
    }
}