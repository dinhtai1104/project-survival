using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullStatusEngine : IStatusEngine
    {
        public ActorBase Owner { get; private set; }

        public bool Lock
        {
            get { return true; }
            set { }
        }

        public void Init(ActorBase actor)
        {
            Owner = actor;
        }

        public void OnUpdate()
        {
        }

        public void SetImmune<T>(bool immune) where T : IStatus
        {
        }

        public void SetImmune(Type type, bool immune)
        {
        }

        public bool IsImmune(Type type)
        {
            return false;
        }

        public void SetImmune(string tag, bool immune)
        {
        }

        public bool IsImmune(string tag)
        {
            return false;
        }

        public bool IsImmune(IList<string> tags)
        {
            return false;
        }

        public int CountStatus(Type type)
        {
            return 0;
        }

        public int CountStatus<T>() where T : IStatus
        {
            return 0;
        }

        public int CountStatus(Type type, ActorBase source)
        {
            return 0;
        }

        public int CountStatus<T>(ActorBase source) where T : IStatus
        {
            return 0;
        }

        public bool HasStatusWithTag(string tag)
        {
            return false;
        }

        public T GetStatus<T>() where T : IStatus
        {
            return default;
        }

        public T GetStatus<T>(ActorBase source) where T : IStatus
        {
            return default;
        }

        public bool HasStatus<T>() where T : IStatus
        {
            return false;
        }

        public bool HasStatus<T>(ActorBase source) where T : IStatus
        {
            return false;
        }

        public bool HasStatus(Type type)
        {
            return false;
        }

        public bool HasStatus(ActorBase source)
        {
            return false;
        }

        public void ClearStatus(IStatus status, bool forced = false)
        {
        }

        public void ClearAllStatus(bool forced = false)
        {
        }

        public void ClearStatuses(string tag, bool forced = false)
        {
        }

        public void ClearStatuses<T>(bool forced = false) where T : IStatus
        {
        }

        public void ClearStatuses(Type type, bool forced = false)
        {
        }

        public void ClearStatuses<T>(ActorBase source, bool forced = false) where T : IStatus
        {
        }

        public void ClearStatuses(ActorBase source, bool forced = false)
        {
        }

        public void AddStatuses(ActorBase source, GameObject[] statuses)
        {
        }

        public IStatus AddStatus(ActorBase source, GameObject statusPrefab, bool forced = false)
        {
            return null;
        }

        public IStatus AddStatusWithoutStart(ActorBase source, GameObject statusPrefab, bool forced = false)
        {
            return null;
        }

        public bool TryAddStatus(ActorBase source, GameObject statusPrefab, out IStatus status, bool forced = false)
        {
            status = null;
            return false;
        }
    }
}