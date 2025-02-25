using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface ITargetFinder
    {
        ActorBase Owner { get; }
        ITargetQuery CurrentQuery { get; }
        ActorBase CurrentTarget { get; set; }
        IList<ActorBase> Enemies { get; }
        IList<ActorBase> Allies { get; }
        bool IsUpdatingTarget { set; get; }
        void Init(ActorBase actor);
        void OnUpdate();
        void UpdateTarget();
        void ForceUpdateTarget();
        void Clear();
        bool ChangeQuery(System.Type queryType);
        bool ChangeQuery<TQuery>() where TQuery : ITargetQuery;
    }
}