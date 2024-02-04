using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorAttackNearestTargetTask : SkillTask
{
    public override async UniTask Begin()
    {

        ITarget target = Caster.Sensor.CurrentTarget;
        if (target != null)
        {
            AimAtTarget(target.GetMidTransform().position, (Character)Caster);
            Caster.AttackHandler.Trigger((target.GetMidTransform().position- Caster.WeaponHandler.DefaultAttackPoint.position).normalized, target);
        }
        await base.Begin();

        IsCompleted = true;
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
