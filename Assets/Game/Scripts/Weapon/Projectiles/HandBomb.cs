using Game.Effect;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBomb : BulletBase
{
    [SerializeField]
    private bool directionFollowVelocity=false;
 
 

    Rigidbody2D rb;
    public override void SetUp(WeaponBase weaponBase, Vector2 triggerPos, Vector2 direction, string playerTag, float offset = 0,Transform target=null, float delay = 0)
    {
        base.SetUp(weaponBase, triggerPos, direction, playerTag, offset,target,delay);
        rb = GetComponent<Rigidbody2D>();
      
        _transform.localEulerAngles = Vector3.zero;
    }
    protected  void FixedUpdate()
    {
        if (directionFollowVelocity)
        {
            _transform.right = rb.velocity;
        }
    }
}
