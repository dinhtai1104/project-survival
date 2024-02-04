using Game.Effect;
using Game.GameActor;
using Game.Pool;
using UnityEngine;
public class BoomerangInstantImpactHandler : InstantImpactHandler
{
    

    public override void Impact(ITarget target)
    {
        if (Base == null) return;
        if ((target != null && (Object)target == this.Base) || (target != null && target.IsDead()))
        {
            return;
        }
        if (target != null && !Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))
        {
            return;
        }
        base.Impact(target);


    }

  


    
}