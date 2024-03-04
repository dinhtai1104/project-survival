using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullBrain : IBrain
    {
        public Actor Owner { get; private set; }
        public bool Lock { get; set; }

        public void Init(Actor actor)
        {
            Owner = actor;
        }

        public void OnUpdate()
        {
        }
    }
}