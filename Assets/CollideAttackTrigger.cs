using Game.GameActor;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideAttackTrigger : MonoBehaviour
{
    ActorBase caster;
    float pushForce;
    float delay;
    public void SetUp(ActorBase caster,Vector2 offScale,float pushForce,float delay)
    {
        this.delay = delay;
        this.pushForce = pushForce;
        this.caster = caster;
        transform.SetParent(caster.GetTransform(),true);
        transform.localPosition = Vector3.zero;
        transform.localScale=new Vector3(1,1,1);
        var collider = transform.parent.GetComponent<Collider2D>();
        var thisCollider = GetComponent<BoxCollider2D>();
        if (collider.GetType() == typeof(BoxCollider2D))
        {
            thisCollider.size = ((BoxCollider2D)collider).size + offScale;
            thisCollider.offset = ((BoxCollider2D)collider).offset;
            thisCollider.edgeRadius = ((BoxCollider2D)collider).edgeRadius;
        }
        if (collider.GetType() == typeof(CapsuleCollider2D))
        {
            thisCollider.size = ((CapsuleCollider2D)collider).size + offScale;
            thisCollider.offset = ((CapsuleCollider2D)collider).offset;
            thisCollider.edgeRadius = 0;
        }
        gameObject.SetActive(true);
    }
    float time;
    [ShowInInspector]
    ITarget target;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ITarget target = collision.GetComponentInParent<ITarget>();
        if(target!=null  &&!target.IsDead() &&caster.AttackHandler.targetType.Contains(target.GetCharacterType()))
        {
            this.target = target;
            Attack((ActorBase)target);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (target != null && collision.GetComponentInParent<ITarget>()==target)
        {
            target = null;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(target!=null && Time.time - time > delay)
        {
            Attack((ActorBase)target);
        }
    }
    protected virtual void Attack(ActorBase target)
    {
        if (caster.Tagger.HasTag(ETag.Immune)) return;
        time = Time.time;

        float damage = caster.Stats.GetValue(StatKey.Dmg);
        DamageSource damageSource = new DamageSource(caster, (ActorBase)target, damage,null);

        var modifier = new ModifierSource(damage);

        damageSource.Value = modifier.Value == 0 ? damage : modifier.Value;
        damageSource._damageSource = EDamageSource.Weapon;

        target.GetHit(damageSource, null);
        //target.MoveHandler.AddForce((target.GetMidTransform().position - caster.GetMidTransform().position).normalized * pushForce);
        target.MoveHandler.AddForce(new Vector2(target.GetMidTransform().position.x > caster.GetMidTransform().position.x?1:-1,0.5f) * pushForce);
    }
}
