using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullTargetFinder : ITargetFinder
    {
        public ActorBase Owner { get; private set; }
        public ITargetQuery CurrentQuery { get; }
        public ActorBase CurrentTarget { get; set; }
        public bool IsUpdatingTarget { get; set; }

        public IList<ActorBase> Enemies => new List<ActorBase>();

        public IList<ActorBase> Allies => new List<ActorBase>();

        public void Init(ActorBase actor)
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