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

        public ActorBase GetTarget(IList<ActorBase> targets)
        {
            return null;
        }

        public void SetTarget(ActorBase target)
        {
        }

        public void ForceUpdateTarget()
        {
        }

        public void OnUpdate()
        {
        }

        public ActorBase GetTarget(IList<ActorBase> targets, params ActorBase[] except)
        {
            return null;
        }
    }
}