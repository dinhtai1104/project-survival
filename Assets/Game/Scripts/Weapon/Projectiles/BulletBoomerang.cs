using Game.Effect;
using Game.GameActor;
using Game.Pool;
using UnityEngine;

public class BulletBoomerang : BulletBase
{

    public override void SetUp(WeaponBase weaponBase, Vector2 triggerPos, Vector2 direction, string playerTag, float offset = 0, Transform target = null,float delay=0)
    {
        base.SetUp(weaponBase, triggerPos, direction, playerTag, offset, target,delay);
        firstImpact = false;

    }
   
  
}