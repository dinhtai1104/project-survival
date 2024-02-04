using Game.GameActor;
using System;
using UnityEngine;

public class ActorBodyAttack : CharacterDamageObject
{
    public ActorBase actorBase;
    public override Stat DmgStat { get => actorBase.Stats.GetStat(StatKey.Dmg); }
    protected override void OnValidate()
    {
        base.OnValidate(); 
        actorBase = GetComponentInParent<ActorBase>();
    }

    private void OnEnable()
    {
        _hit.onTrigger += OnTrigger;
    }
    private void OnDisable()
    {
        _hit.onTrigger -= OnTrigger;
    }

    private void OnTrigger(Collider2D collider,ITarget target)
    {
        //var actor = collider.GetComponent<ActorBase>();
        if (target != null)
        {
            if (target.GetCharacterType() != actorBase.GetCharacterType())
            {
                var dmgSource = new DamageSource
                {
                    Attacker = actorBase,
                    Defender = (ActorBase)target,
                    _damage = DmgStat
                };
                dmgSource._damageSource = EDamageSource.Weapon;
                target.GetHit(dmgSource, (IDamageDealer)target);
            }
        }
    }
}