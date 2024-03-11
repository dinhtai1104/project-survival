using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface ITargetFinder
    {
        Actor Owner { get; }
        ITargetQuery CurrentQuery { get; }
        Actor CurrentTarget { get; set; }
        IList<Actor> Enemies { get; }
        IList<Actor> Allies { get; }
        bool IsUpdatingTarget { set; get; }
        void Init(Actor actor);
        void OnUpdate();
        void UpdateTarget();
        void ForceUpdateTarget();
        void Clear();
        bool ChangeQuery(System.Type queryType);
        bool ChangeQuery<TQuery>() where TQuery : ITargetQuery;
    }
}