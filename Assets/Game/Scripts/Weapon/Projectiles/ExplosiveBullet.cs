using Game.Effect;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : BulletBase
{
    //[SerializeField]
    //private ValueConfigSearch explodeRadius;
    //[SerializeField]
    //private LayerMask impactMask;
    //protected override void Impact(Collider2D collision)
    //{
    //    if (collision != null && gameObject.activeSelf)
    //    {
    //        Explode();
    //    }

    //}
   
    //Collider2D[] colliders = new Collider2D[10];
    //void Explode()
    //{
    //    int count = Physics2D.OverlapCircleNonAlloc(_transform.position, explodeRadius.FloatValue, colliders, impactMask);
    //    for (int i = 0; i < count; i++)
    //    {
    //        ITarget target = colliders[i].GetComponentInParent<ITarget>();

    //        if (target != null && (Object)target != weaponBase.character)
    //        {
    //            DamageSource damageSource = new DamageSource(character, (ActorBase)target, weaponBase.GetDamage());
    //            target.GetHit(damageSource, this);
    //        }
    //    }
    //    Vector3 pos = _transform.position;
    //    GameObjectSpawner.Instance.Get(impactEffect, res =>
    //    {
    //        res.GetComponent<EffectAbstract>().Active(pos);
    //    });

    //    gameObject.SetActive(false);
    //}
}
