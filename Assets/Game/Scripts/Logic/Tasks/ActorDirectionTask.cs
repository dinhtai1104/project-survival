using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorDirectionTask : SkillTask
{
    public Vector2 direction;
    public override async UniTask Begin()
    {
        await base.Begin();
      
        //Logger.Log("Attack+>>>>> " + Caster.gameObject.name + " " + (target.GetTransform().gameObject.name));
        Caster.AttackHandler.Trigger(direction,Caster.Sensor.CurrentTarget);
        if(direction.x!=0)
            ((Character)Caster).SetFacing(direction.x);
        IsCompleted = true; 
    }
}