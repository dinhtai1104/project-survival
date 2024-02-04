using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;
public class BulletSimpleDamageObject : CharacterDamageObject
{
    private int maxHit = 1;
    private int currentMaxHit = 0;
    public override void Play()
    {
        currentMaxHit = 0;
        base.Play();
        _hit.onTrigger -= OnTriggerHit;
        _hit.onTrigger += OnTriggerHit;
        
    }

    public virtual void SetMaxHit(int maxHit)
    {
        _hit.SetIsFullTimeHit(maxHit == -1);
        _hit.SetMaxHit(maxHit);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _hit.onTrigger -= OnTriggerHit;
    }
    public virtual void SetMaxHitToTarget(int maxHit)
    {
        this.maxHit = maxHit;
    }

    protected virtual void OnTriggerHit(Collider2D collider, ITarget target)
    {
     
            //Logger.Log("=>>> "+collider.gameObject.name+" "+(target!=null?target.ToString():"null"));
        if (target != null && (UnityEngine.Object)target!=Caster && Caster.AttackHandler.IsValid(target.GetCharacterType()))
        {
            currentMaxHit++;
            Impact(target);
            if (currentMaxHit >= maxHit)
            {
                var _beforeDestroyObject = new List<IBeforeDestroyObject>(GetComponents<IBeforeDestroyObject>());
                foreach (var beforeDestroyObject in _beforeDestroyObject)
                {
                    beforeDestroyObject.Action(collider);
                }
                if (Poolable)
                {
                    PoolManager.Instance.Despawn(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    protected virtual void Impact(ITarget actor)
    {
        //Logger.Log("GET HIT:" + DmgStat.BaseValue + " " + DmgStat.Value);
        var damageSource = new DamageSource
        {
            Attacker = Caster,
            Defender = (ActorBase)actor,
            _damage = DmgStat,
            posHit = transform.position,
            _damageSource = EDamageSource.Weapon,
        };
        actor.GetHit(damageSource, (IDamageDealer)actor);
    }
}
