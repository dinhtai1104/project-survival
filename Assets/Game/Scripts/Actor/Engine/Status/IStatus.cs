using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface IStatus
    {
        ITagger Tagger { get; }
        bool Permanent { get; }
        bool Expirable { get; }
        bool Stackable { get; }
        int MaxStack { get; }
        bool Override { get; }
        bool IsExpired { get; }
        bool IsRunning { get; }
        Actor Actor { get; }
        Actor Source { get; }
        void Begin();
        void Cancel();
        void Stop();
        void OnUpdate(float dt);
        void SetActor(Actor actor);
        void SetSource(Actor source);
        IStatus SetDuration(float duration);
        IStatus SetModifierValue(float value);
    }
}