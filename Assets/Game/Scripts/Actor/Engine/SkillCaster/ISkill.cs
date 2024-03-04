using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public interface ISkill
    {
        int Id { get; }
        int Priority { get; }
        bool Ignore { set; get; }
        bool Interruptible { set; get; }
        float Cooldown { set; get; }
        bool AutoCast { set; get; }
        bool IsLocked { set; get; }
        bool IsCooldown { get; }
        string CastingAnimation { get; }
        float CastingRange { get; }
        bool InvokeCastingSkillManually { set; get; }
        bool StartCooldownManually { set; get; }
        bool LockOtherSkill { set; get; }
        bool IsCooldownTimerRunning { get; }
        bool IsPassive { set; get; }
        float RemainingCooldownTime { get; }
        bool CanCast { get; }
        float StartingCooldown { set; get; }
        bool CastNoCooldown { set; get; }
        bool IsExecuting { set; get; }
        Actor Caster { get; }
        void Init(Actor actor);
        void OnUpdate();
        void ResetCooldown();
        void FasterCooldown(float duration);
        void StartCooldown();
        void Cast();
        void InvokeCastingSkill();
        void Reset();
        void Stop();
        void SetActiveCooldownTimer(bool active);
        UnityEvent OnCastingEvent { get; }
        UnityEvent OnExitEvent { get; }
    }
}