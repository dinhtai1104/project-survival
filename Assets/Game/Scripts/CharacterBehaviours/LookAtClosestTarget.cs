using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/LookAtClosestTarget")]
public class LookAtClosestTarget : CharacterBehaviour
{
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        LookAtClosestTarget instance = (LookAtClosestTarget)base.SetUp(character);
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    float time = 0;
    ITarget target = null;                                                                  
    public override void OnUpdate(ActorBase ActorBase)
    {
        Character character = (Character)ActorBase;
        if (!active) return;
       
        if (Time.time - time > 0.1f)
        {
            target = ActorBase.Sensor.CurrentTarget;

        
            if (target!=null)
            {
                AimAtTarget(target.GetMidTransform().position);
            }
            time = Time.time;

        }
        else
        {
           
            if (target!=null )
            {
                AimAtTarget(target.GetMidTransform().position);
            }
            else
            {
                character.SetFacing(character.MoveHandler.lastMove.x >= 0 ? 1 : -1);
                character.SetLookDirection(0, 0);
            }
        }

        void AimAtTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition- character.GetMidTransform().position).normalized;
            character.SetLookDirection(0, 0);
            character.SetFacing(direction.x > 0 ? 1 : -1);
            character.SetLookDirection(direction.x, direction.y);
        }

    }
}
