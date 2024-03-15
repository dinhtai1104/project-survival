using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullBrain : IBrain
    {
        public ActorBase Owner { get; private set; }
        public bool Lock { get; set; }

        public void Init(ActorBase actor)
        {
            Owner = actor;
        }

        public void OnUpdate()
        {
        }
    }
}