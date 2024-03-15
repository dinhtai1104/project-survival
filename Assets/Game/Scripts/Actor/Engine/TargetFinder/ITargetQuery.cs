using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface ITargetQuery
    {
        void Init(ITargetFinder finder);
        ActorBase GetTarget(IList<ActorBase> targets);
        ActorBase GetTarget(IList<ActorBase> targets, params ActorBase[] except);
        void SetTarget(ActorBase target);
        void ForceUpdateTarget();
        void OnUpdate();
    }
}