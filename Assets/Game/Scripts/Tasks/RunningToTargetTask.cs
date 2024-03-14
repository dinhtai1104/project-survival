using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionKit;

public class RunningToTargetTask : SkillTask
{
    [SerializeField, Range(0f, 10f)] private float m_TimeOut;
    [SerializeField] private string m_MovingAnimation;
    [SerializeField] private string m_IdleAnimation;
    [SerializeField] private bool m_LoopAnimation = true;
    [SerializeField] private bool m_SyncTimeScale;
    [SerializeField] private bool m_Follow;
    [SerializeField] private float m_RangeOffset;
    [SerializeField] private bool m_CompleteIfInRange;
    [SerializeField] private ModifierData m_SpeedMod;

    private float m_Range;

    private bool m_ReachTarget;
    private float m_CurrentOffsetY;
    private float m_AimHeight;
    private Vector3 m_Dest;
    private float m_TimeOutTimer;

    public override void Begin()
    {
        base.Begin();
        m_ReachTarget = false;
        m_TimeOutTimer = 0f;
        Caster.Stats.AddModifier(m_SpeedMod.AttributeName, m_SpeedMod.Modifier, this);
    }

    public override void End()
    {
        base.End();
        Caster.Movement.IsMoving = false;
        Caster.Animation.Play(0, m_IdleAnimation);
        m_Range = Caster.Stats.GetValue(StatKey.AttackRange) + m_RangeOffset;
        if (Caster != null)
        {
            Caster.Stats.RemoveModifier(m_SpeedMod.AttributeName, m_SpeedMod.Modifier);
        }

        if (m_SyncTimeScale)
        {
            Caster.Animation.TimeScale = 1f;
        }
    }

    public override void Interrupt()
    {
        base.Interrupt();
        Caster.Movement.IsMoving = false;
        Caster.Animation.Play(0, m_IdleAnimation);
        if (Caster != null)
        {
            Caster.Stats.RemoveModifier(m_SpeedMod.AttributeName, m_SpeedMod.Modifier);
        }

        IsCompleted = true;
    }

    public override void Run()
    {
        base.Run();
        if (Caster.Movement.LockMovement)
        {
            IsCompleted = true;
            return;
        }

        if (m_TimeOut > 0f)
        {
            m_TimeOutTimer += Time.deltaTime;
            if (m_TimeOutTimer >= m_TimeOut)
            {
                ForceInterruptTask = true;
                return;
            }
        }

        var target = Caster.TargetFinder.CurrentTarget;
        if (target != null)
        {
            var targetPos = target.Trans.position;
            var casterPos = Caster.Trans.position;
            if (!m_ReachTarget)
            {
                if (m_MovingAnimation.IsNotNullAndEmpty())
                {
                    Caster.Animation.Play(0, m_MovingAnimation, m_LoopAnimation);
                    if (m_SyncTimeScale)
                    {
                        var stats = Caster.Stats;
                        Caster.Animation.TimeScale = stats.GetValue(StatKey.Speed) / stats.GetBaseValue(StatKey.Speed);
                    }
                }

                if (Caster.AI) Caster.Movement.LookAt(targetPos);

                var diffVec = targetPos - casterPos;
                var distX = Mathf.Abs(diffVec.x);
                diffVec.z = 0f;

                if (distX <= m_Range)
                {
                    if (m_CompleteIfInRange)
                    {
                        IsCompleted = true;
                        return;
                    }

                    m_Dest = casterPos;
                    Caster.Movement.MoveTo(m_Dest);

                    if (m_Follow)
                    {
                        return;
                    }

                    m_ReachTarget = true;
                    IsCompleted = true;
                }
                else
                {
                    Caster.Movement.MoveTo(targetPos);
                }
            }
        }
        else
        {
            if (!m_Follow && Caster.AI)
            {
                IsCompleted = true;
            }
        }
    }
}