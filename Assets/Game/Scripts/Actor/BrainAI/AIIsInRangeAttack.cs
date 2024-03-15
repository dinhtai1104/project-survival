using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AIIsInRangeAttack : BrainDecision
{
    public override bool Decide(ActorBase actor)
    {
        var target = actor.TargetFinder.CurrentTarget;
        if (target == null) return false;
        var dist = (target.CenterPosition - actor.CenterPosition).magnitude;
        if(dist < actor.Stats.GetValue(StatKey.AttackRange))
        {
            return true;
        }
        return false;
    }
}
