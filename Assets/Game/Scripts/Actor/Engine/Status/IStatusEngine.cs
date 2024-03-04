using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface IStatusEngine
    {
        Actor Owner { get; }
        bool Lock { set; get; }
        void Init(Actor actor);
        void OnUpdate();
        void SetImmune<T>(bool immune) where T : IStatus;

        void SetImmune(Type type, bool immune);

        bool IsImmune(Type type);

        void SetImmune(string tag, bool immune);

        bool IsImmune(string tag);

        bool IsImmune(IList<string> tags);

        int CountStatus(Type type);

        int CountStatus<T>() where T : IStatus;

        int CountStatus(Type type, Actor source);

        int CountStatus<T>(Actor source) where T : IStatus;

        bool HasStatusWithTag(string tag);
        T GetStatus<T>() where T : IStatus;

        T GetStatus<T>(Actor source) where T : IStatus;

        bool HasStatus<T>() where T : IStatus;

        bool HasStatus<T>(Actor source) where T : IStatus;

        bool HasStatus(Type type);

        bool HasStatus(Actor source);
        void ClearStatus(IStatus status, bool forced = false);

        void ClearAllStatus(bool forced = false);

        void ClearStatuses(string tag, bool forced = false);

        void ClearStatuses<T>(bool forced = false) where T : IStatus;
        void ClearStatuses(Type type, bool forced = false);
        void ClearStatuses<T>(Actor source, bool forced = false) where T : IStatus;
        void ClearStatuses(Actor source, bool forced = false);
        void AddStatuses(Actor source, GameObject[] statuses);
        IStatus AddStatus(Actor source, GameObject statusPrefab, bool forced = false);
        IStatus AddStatusWithoutStart(Actor source, GameObject statusPrefab, bool forced = false);
        bool TryAddStatus(Actor source, GameObject statusPrefab, out IStatus status, bool forced = false);
    }
}
