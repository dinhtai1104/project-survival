using Game.Effect;
using Game.GameActor;
using Game.Pool;
using UnityEngine;

public class ExplosiveImpactHandler : ImpactHandler
{
    [SerializeField]
    private ValueConfigSearch explodeRadius;
    [SerializeField]
    private LayerMask impactMask;

    

  
   
    public override void Impact(ITarget target)
    {
        
        if ((Object)target == Base.Caster || (target!=null && !Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))) return;
        Explode();
    }
    public override void ForceImpact()
    {

        Explode(true);
    }

    Collider2D[] colliders = new Collider2D[10];
    void Explode(bool force=false)
    {
        int count = Physics2D.OverlapCircleNonAlloc(Base.GetTransform().position, explodeRadius.SetId(Base.weaponBase.character.gameObject.name).FloatValue, colliders, impactMask);
        bool isExploded = count>0 ||force;
        
        for (int i = 0; i < count; i++)
        {
            ITarget target = colliders[i].GetComponentInParent<ITarget>();

            if (target != null && (Object)target != Base.Caster && Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))
            {
                DamageSource damageSource = new DamageSource(Base.Caster, (ActorBase)target, Base.DmgStat.Value, Base);
                damageSource.posHit = transform.position;
                damageSource._damageSource = EDamageSource.Effect;
                target.GetHit(damageSource, Base);
                isExploded = true;

                ApplyStatus(target);
            }
        }

        if (isExploded)
        {
            Vector3 pos = Base.GetTransform().position;
            GameObjectSpawner.Instance.Get(impactEffect, res =>
            {
                res.GetComponent<EffectAbstract>().Active(pos);
            });
            gameObject.SetActive(false);
        }

    }



}
