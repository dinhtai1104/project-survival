using Game.GameActor;
using System;
using UnityEngine;

public class ShieldObject : ObjectBase
{
    public ParticleSystem effShield;
    private ActorBase caster;
    private float dmgDecreasePercent = 0;
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }

    private void OnBeforeHit(ActorBase attacker, ActorBase defender, DamageSource damageSource)
    {
        if (defender != caster) return;
        effShield.Stop();
        effShield.Play();
        var posHit = damageSource.posHit;
        if (GameUtility.GameUtility.GetRange(defender.GetMidTransform().position, posHit) > 3) { return; }

        // Check direction
        var defenderDirect = Mathf.Sign(defender.GetLookDirection().x);
        var side = defender.GetMidTransform().position.x <= posHit.x ? 1 : -1;

        if (defenderDirect == side)
        {
            damageSource.AddModifier(new StatModifier(EStatMod.PercentAdd, -dmgDecreasePercent));
        }
    }
    public void SetDmgDecrease(float dmgDecreasePercent)
    {
        this.dmgDecreasePercent = dmgDecreasePercent;
    }
    public void SetActor(ActorBase caster)
    {
        this.caster = caster;
    }
}