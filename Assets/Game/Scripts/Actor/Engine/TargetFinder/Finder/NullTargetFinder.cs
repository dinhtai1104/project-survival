using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullTargetFinder : ITargetFinder
    {
        public Actor Owner { get; private set; }
        public ITargetQuery CurrentQuery { get; }
        public Actor CurrentTarget { get; set; }
        public bool IsUpdatingTarget { get; set; }

        public void Init(Actor actor)
        {
            Owner = actor;
        }

        public void OnUpdate()
        {
        }

        public void UpdateTarget()
        {
        }

        public void ForceUpdateTarget()
        {
        }

        public void Clear()
        {
        }

        public bool ChangeQuery(Type queryType)
        {
            return false;
        }

        public bool ChangeQuery<TQuery>() where TQuery : ITargetQuery
        {
            return false;
        }
    }
}