using Game.GameActor;
using System;
using UnityEngine;

public class HunterNecklacePassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicCooldown;
    public ValueConfigSearch legendaryCooldown;

    private float cooldown = 0;
    private float currentCooldown = 0;
    private bool canShotCrit = true;
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
    }

    private void OnStageStart(Callback callback)
    {
        canShotCrit = true;
        currentCooldown = 0;
    }

    private void OnBeforeHit(ActorBase attacker, ActorBase defender, DamageSource damageSource)
    {
        if (attacker != Caster) return;
        if (Rarity < ERarity.Epic) return;
        if (!canShotCrit) return;
        Debug.Log("[Passive Hunter Necklace] Appied CritDamage");
        damageSource.IsCrit = true;
        canShotCrit = false;
    }
    public override void Play()
    {
        base.Play();
        canShotCrit = true;
        cooldown = 0;
        if (Rarity >= ERarity.Epic)
        {
            cooldown = epicCooldown.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            cooldown = legendaryCooldown.FloatValue;
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (canShotCrit) return;
        currentCooldown += Time.deltaTime;
        if (currentCooldown >= cooldown)
        {
            currentCooldown = 0;
            canShotCrit = true;
        }
    }
}