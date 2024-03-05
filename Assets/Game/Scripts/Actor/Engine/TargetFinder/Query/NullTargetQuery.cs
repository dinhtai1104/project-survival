using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullTargetQuery : ITargetQuery
    {
        public void Init(ITargetFinder finder)
        {
        }

        public Actor GetTarget(IList<Actor> targets)
        {
            return null;
        }

        public void SetTarget(Actor target)
        {
        }

        public void ForceUpdateTarget()
        {
        }

        public void OnUpdate()
        {
        }
    }
}