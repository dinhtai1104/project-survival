using Effect;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : BulletBase, ITarget
{
    bool isDead = false;
    [SerializeField]
    private int maxHit=2;
    [SerializeField]
    private float blastRadius = 2;
    private int hit = 0;
    [SerializeField]
    private AudioClip explodeSFX;
    Collider2D[] colliders = new Collider2D[5];
    void OnEnable()
    {
        hit = maxHit;
        isDead = false;
    }
    public float GetHealthPoint()
    {
        return hit;
    }
    public bool IsThreat()
    {
        return true;
    }
    public bool CanFocusOn()
    {
        return true;
    }
   
    void Explode()
    {
        Vector3 position = transform.position;
        //EffectSpawner.Instance.Get(impactEffect, res =>
        //{
        //    if (character == null) return;
        //    res.Active(position);
        //    Sound.Controller.Instance.PlayOneShot(explodeSFX, 0.7f);
        //    int count = Physics2D.OverlapCircleNonAlloc(position, blastRadius, colliders, mask);
        //    for (int i = 0; i < count; i++)
        //    {
        //        ITarget target = colliders[i].GetComponentInParent<ITarget>();
        //        target.GetHit(character, (int)character.Stats.GetValue(StatKey.Dmg), this);
        //        colliders[i] = null;
        //    }
        //    //
        //    gameObject.SetActive(false);
        //}).Forget();
      
    }
    public Transform GetMidTransform()
    {
        return transform;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsDead()
    {
        return isDead;
    }

    //protected override void Impact(Collider2D collision)
    //{
    //    Explode();
    //}

    public void HighLight(bool active)
    {
    }

    public Game.GameActor.ECharacterType GetCharacterType()
    {
        return Game.GameActor.ECharacterType.Object;
    }


    public bool GetHit(DamageSource damageSource, IDamageDealer dealer)
    {
        if (isDead) return false;
        hit--;
        if (hit <= 0)
        {
            isDead = true;
            Explode();
        }

        return true;
    }

    public Vector3 GetMidPos()
    {
        return transform.position;
    }
}
