using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorRepeatAttackNearestTargetTask : SkillTask
{
    public ValueConfigSearch TotalAttack ;
    public ValueConfigSearch Delay;
    public override async UniTask Begin()
    {


        attackCount = TotalAttack.IntValue;
        delay = Delay.FloatValue;
        time = 0;
        await base.Begin();
    }
    float time = 0,delay;
    int attackCount = 0;
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        
        if (attackCount > 0)
        {
            if (time >= delay)
            {
                ITarget target = Caster.Sensor.CurrentTarget;
                if (target != null)
                {
                    AimAtTarget(target.GetMidTransform().position, (Character)Caster);
                    Caster.AttackHandler.Trigger((target.GetMidTransform().position - Caster.GetMidTransform().position).normalized, target);
                
                    attackCount--;
                }
                else
                {
                    IsCompleted = true;
                }
                time = 0;
            }
            else
            {
                time += Time.deltaTime;
            }
        }
        else
        {
            IsCompleted = true;
        }
            
    }
    void AimAtTarget(Vector3 targetPosition, Character character)
    {
        Vector3 direction = (targetPosition - character.GetMidTransform().position).normalized;
        character.SetLookDirection(0, 0);
        character.SetFacing(direction.x > 0 ? 1 : -1);
        character.SetLookDirection(direction.x, direction.y);
        //Debug.DrawLine(character.GetMidTransform().position, targetPosition, Color.yellow, 0.1f);

    }
}
