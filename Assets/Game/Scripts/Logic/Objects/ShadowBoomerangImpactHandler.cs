using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBoomerangImpactHandler : MonoBehaviour,IDamageDealer
{
    public ValueConfigSearch Radius , ImpactPerSecond, Duration,DmgMultiply ;
    private float radius = 2, impactPerSecond = 3, duration = 1f,dmgMul=1;

    List<ModifierSource> modifiers = new List<ModifierSource>();
    Transform _transfrom;
    ActorBase Caster;
    Stat DmgStat { set; get; }

    public void SetUp(BulletBase bulletBase)
    {
        _transfrom = transform;
        _transfrom.position = bulletBase.transform.position;
        Caster = bulletBase.Caster;
        DmgStat = new Stat(bulletBase.DmgStat.Value);

        if (modifiers.Count == 0)
        {
            modifiers.Add(new ModifierSource(Radius.FloatValue));
            modifiers.Add(new ModifierSource(ImpactPerSecond.FloatValue));
            modifiers.Add(new ModifierSource(Duration.FloatValue));
            modifiers.Add(new ModifierSource(DmgMultiply.FloatValue));
        }
        else
        {
            foreach(var modifer in modifiers)
            {
                modifer.Stat.ClearModifiers();
            }
        }
        Messenger.Broadcast(EventKey.OnBoomerangShoot, bulletBase.Caster,modifiers);
        foreach (var modifer in modifiers)
        {
            modifer.Stat.RecalculateValue();
        }
        radius = modifiers[0].Value;
        impactPerSecond = modifiers[1].Value;
        duration = modifiers[2].Value;
        dmgMul = modifiers[3].Value;

        aliveTime = 0;
        gameObject.SetActive(true);

        time = Time.time;
    }
  


    float time = 0, aliveTime = 0;
    protected void Update()
    {
        Logger.Log((Time.time - time) +" "+(Time.time - time >= 1f / impactPerSecond));
        if (Time.time - time >= 1f / impactPerSecond)
        {
            {
                ApplySawAttack();
                time = Time.time;
            }
               
        }
        if (aliveTime < duration)
        {
            aliveTime += Time.deltaTime;
        }
        else
        {
            Game.Pool.GameObjectSpawner.Instance.Get("DisableEffect", obj =>
            {
                obj.GetComponent<Game.Effect.EffectAbstract>().Active(_transfrom.position);
            });
            gameObject.SetActive(false);
        }
    }
    Collider2D[] colliders = new Collider2D[5];
    void ApplySawAttack()
        { 
        int count = Physics2D.OverlapCircleNonAlloc(_transfrom.position, radius, colliders, Caster.AttackHandler.targetMask);
        for (int i = 0; i < count; i++)
        {
            ITarget target = colliders[i].GetComponentInParent<ITarget>();
            if ((target != null && (Object)target == Caster) || (target != null && target.IsDead()))
            {
                return;
            }
            if (target != null && !Caster.AttackHandler.IsValid(target.GetCharacterType()))
            {
                return;
            }
            float damage = DmgStat.Value * dmgMul;
            if (target != null && damage!=0)
            {
                DamageSource damageSource = new DamageSource(Caster, (ActorBase)target, damage,this);



                var modifier = new ModifierSource(damage);

                damageSource._damage.RecalculateValue();
                damageSource.Value = modifier.Value == 0 ? damage : modifier.Value;


                damageSource.posHit = transform.position;
                damageSource._damageSource = EDamageSource.Weapon;
                target.GetHit(damageSource, this);
            }
        }
    }

    public Transform GetTransform()
    {
        return _transfrom;
    }

    public Vector3 GetDamagePosition()
    {
        return _transfrom.position;
    }
}