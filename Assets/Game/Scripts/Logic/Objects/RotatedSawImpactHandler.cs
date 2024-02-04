using Game.GameActor;
using UnityEngine;

public class RotatedSawImpactHandler : ImpactHandler
{

    bool isSawActivated = false;
    [SerializeField]
    private LayerMask impactMask;

    [SerializeField]
    private float hitSpeed = 0.2f, radius = 1;
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - startTime < ExplodeDelay) return;

        ITarget target = collision.GetComponentInParent<ITarget>();
        if (isSawActivated && (target==null || (target!=null && Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))))
        {
            //if target collide with obj, apply attack
            if(target!=null && (Object)target != Base.Caster)
            {
                ApplySawAttack();
            }

             return;
        }


        if ((Object)target == Base.Caster || ( target!=null&&!Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))) return;
        
        // if impact with anything, active saw now
        ((RotatedSawBullet)Base).ActiveSaw();

    }
  
    public override void Impact(ITarget target)
    {
        //time = 0;
        isSawActivated = true;
        ApplySawAttack();
    }
  

    public override void SetUp(BulletBase bulletBase)
    {
        base.SetUp(bulletBase);
         isSawActivated = false;
    }

   
    Collider2D[] colliders = new Collider2D[5];
    void ApplySawAttack()
    {
        int count = Physics2D.OverlapCircleNonAlloc(Base.GetTransform().position, radius, colliders, impactMask);
        for (int i = 0; i < count; i++)
        {
            ITarget target = colliders[i].GetComponentInParent<ITarget>();

            if (target != null && (Object)target != Base.Caster &&  Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))
            {
                DamageSource damageSource = new DamageSource(Base.Caster, (ActorBase)target, Base.DmgStat.Value, Base);
                damageSource.posHit = transform.position;
                damageSource._damageSource = EDamageSource.Weapon;

                target.GetHit(damageSource, Base);
            }
        }
    }
}