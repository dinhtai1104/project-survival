using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/PointGunAtTarget")]
public class PointGunAtTarget : CharacterBehaviour
{
  
    public override void OnActive(ActorBase character)
    {
        active = true;
    }

    public override void OnUpdate(ActorBase ActorBase)
    {
        if (!active) return;
        Transform aimTransform = ActorBase.GetAimTransform();
        
        ((GunBase)ActorBase.WeaponHandler.CurrentWeapon).weaponObj.Rotate(ActorBase.GetLookDirection());
        ((GunBase)ActorBase.WeaponHandler.CurrentWeapon).weaponObj.Flip(ActorBase.GetLookDirection().x);

        int index = 1;
        foreach (var supportWeapon in ActorBase.WeaponHandler.SupportWeapons)
        {
            ITarget trackingTarget = ActorBase.Sensor.GetTarget(index);
            Vector2 direction=trackingTarget!=null?(Vector2)(trackingTarget.GetMidTransform().position-ActorBase.WeaponHandler.GetAttackPoint(supportWeapon).position).normalized: ActorBase.GetLookDirection();
            ((GunBase)supportWeapon).weaponObj.Rotate(direction);
            ((GunBase)supportWeapon).weaponObj.Flip(direction.x);
        }

        // prevent facing too down below or above
        Vector3 aimDirection = ActorBase.GetLookDirection();
        if (aimDirection.x >= 0 && aimDirection.x < 0.8f) aimDirection.x = 0.8f;
        if (aimDirection.x < 0 && aimDirection.x > -0.8f) aimDirection.x = -0.8f;
        //}
        // player look at target
        aimTransform.position = ActorBase.GetLookTransform().position + aimDirection;

    }
}
