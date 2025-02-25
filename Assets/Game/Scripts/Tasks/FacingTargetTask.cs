using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class FacingTargetTask : SkillTask
{
    [SerializeField] private bool m_Once = true;

    public override void Begin()
    {
        base.Begin();
        if (m_Once)
        {
            ActorBase target = Caster.TargetFinder.CurrentTarget;
            if (target != null)
            {
                Caster.Movement.LookAt(target.Trans.position);
            }

            IsCompleted = true;
        }
    }

    public override void Run()
    {
        base.Run();
        if (!m_Once)
        {
            ActorBase target = Caster.TargetFinder.CurrentTarget;
            if (target != null)
            {
                Caster.Movement.LookAt(target.Trans.position);
            }
        }
    }
}