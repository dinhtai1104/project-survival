using Game.Effect;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : BulletBase
{
    [SerializeField]
    private bool directionFollowVelocity = false;
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
    //    bool isExploded = false;
    //    for (int i = 0; i < count; i++)
    //    {
    //        ITarget target = colliders[i].GetComponentInParent<ITarget>();
    //        if (target != null && !character.AttackHandler.IsValid(target.GetCharacterType()))
    //        {
    //            continue;
    //        }
    //        if (target != null && (Object)target != weaponBase.character )
    //        {
    //            DamageSource damageSource = new DamageSource(character, (ActorBase)target, weaponBase.GetDamage());
    //            damageSource.posHit = transform.position;
    //            target.GetHit(damageSource, this);
    //            isExploded = true;
    //        }
    //    }
    //    if (isExploded)
    //    {
    //        Vector3 pos = _transform.position;
    //        GameObjectSpawner.Instance.Get(impactEffect, res =>
    //        {
    //            res.GetComponent<EffectAbstract>().Active(pos);
    //        });

    //        gameObject.SetActive(false);
    //    }
    //}

    Rigidbody2D rb;
    public override void SetUp(WeaponBase weaponBase, Vector2 triggerPos, Vector2 direction, string playerTag, float offset = 0, Transform target = null, float delay = 0)
    {
        base.SetUp(weaponBase, triggerPos, direction, playerTag, offset, target,delay);
        rb = GetComponent<Rigidbody2D>();
        _transform.localEulerAngles = Vector3.zero;
    }
    protected void FixedUpdate()
    {
        if (directionFollowVelocity)
        {
            _transform.right = rb.velocity;
        }
    }
}
