using Game.GameActor;
using System;
using UnityEngine;

[RequireComponent(typeof(InvokeHitByTrigger))]
public class Boss20Skill3Leaf : CharacterObjectBase, IDamage
{
    public Stat DmgStat { get; set; }
    public override void Play()
    {
        base.Play();
        var hit = GetComponent<InvokeHitByTrigger>();
        hit.SetIsFullTimeHit(false);
        hit.SetMaxHit(1);
        hit.onTrigger += OnTrigger;
    }

    private void OnTrigger(Collider2D collider, ITarget target)
    {
        if (target != null)
        {
            Impact(target as ActorBase);
        }
    }
    private void Impact(ActorBase actor)
    {
        if (actor.GetCharacterType() != Caster.GetCharacterType())
        {
            var damageSource = new DamageSource
            {
                Attacker = Caster,
                Defender = actor,
                _damage = DmgStat
            };
            damageSource.posHit = transform.position;
            damageSource._damageSource = EDamageSource.Weapon;
            actor.GetHit(damageSource, this);
        }
    }
}
