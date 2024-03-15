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
        bool InvokeCastingSkillManually { set; get; }
        bool StartCooldownManually { set; get; }
        bool LockOtherSkill { set; get; }
        bool IsCooldownTimerRunning { get; }
        float RemainingCooldownTime { get; }
        bool CanCast { get; }
        float StartingCooldown { set; get; }
        bool CastNoCooldown { set; get; }
        bool IsExecuting { set; get; }
        ActorBase Caster { get; }
        void Init(ActorBase actor);
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

        void SetManuallyCooldown(Stat cooldown);
        void SetManuallyStartCooldown(Stat cooldown);
    }
}