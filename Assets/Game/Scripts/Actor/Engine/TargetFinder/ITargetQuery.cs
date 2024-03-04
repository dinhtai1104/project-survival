using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface ITargetQuery
    {
        void Init(ITargetFinder finder);
        Actor GetTarget();
        void SetTarget(Actor target);
        void ForceUpdateTarget();
        void OnUpdate();
    }
}