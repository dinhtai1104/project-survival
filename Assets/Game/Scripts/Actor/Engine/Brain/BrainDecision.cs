using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public abstract class BrainDecision : ScriptableObject
    {
        public abstract bool Decide(ActorBase actor);
    }
}