using Game.Effect;
using Game.GameActor;
using Game.Pool;
using UnityEngine;
public class InstantImpactHandler: ImpactHandler
{
    

    public override void Impact(ITarget target)
    {

        if (Base == null) return;
        if ((target != null && (Object)target == this.Base) || (target != null && target.IsDead()))
        {
            return;
        }
        if (target != null && !Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))
        {
            return;
        }
        if (target != null && Base.DmgStat.Value!=0 && target.GetCharacterType()!=ECharacterType.Object)
        {
            float damage = Base.DmgStat.Value;
            
            DamageSource damageSource = new DamageSource(Base.Caster, (ActorBase)target, damage, Base);
            damageSource._damageSource = EDamageSource.Weapon;


            var modifier = new ModifierSource(damage);

            Messenger.Broadcast(EventKey.BulletImpact, Base, modifier);

            damageSource.Value = modifier.Value == 0 ? damage : modifier.Value;


            damageSource.posHit = transform.position;
            target.GetHit(damageSource, Base);


        }
        ApplyStatus(target);



        Vector3 pos = Base.GetTransform().position;
        GameObjectSpawner.Instance.Get(impactEffect, res =>
        {
            res.GetComponent<EffectAbstract>().Active(pos);
        });
        
       
        bool check = false;
        if (target!=null&&Pierce())
        {
            Base.currentPiercing++;
            check = true;
        }
         if (target==null & Bounce())
        {
            Base.currentBounce++;
            check = true;
        }

         if (target!=null &&Ricochet())
        {
            Base.currentRicochet++;
            check = true;
        }

         if(Base.destroyOnImpact && !check)
        {
            gameObject.SetActive(false);
        }

        Messenger.Broadcast(EventKey.OnBulletImpact, Base,target);

    }
    protected bool Ricochet()
    {
        if (Base.canRicochet &&Base.currentRicochet>=0 &&Base.currentRicochet < Base.MaxRicochet)
        {
            return true;
        }
        return false;
    }


    protected bool Pierce()
    {
        if (Base.currentPiercing < Base.MaxPiercing)
        {
            return true;
        }
        return false;
    }


    public bool Bounce()
    {
        if (Base.currentBounce <Base.MaxBounce)
        {
            return true;
        }
        return false;
    }
  


    
}