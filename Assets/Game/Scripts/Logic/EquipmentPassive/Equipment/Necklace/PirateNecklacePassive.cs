using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class PirateNecklacePassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicDmgExtra;
    public ValueConfigSearch legendaryDmgExtra;

    private float extra;
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    public override void Play()
    {
        base.Play();
        extra = 0;
        if (itemEquipment.EquipmentRarity >= ERarity.Epic)
        {
            extra += epicDmgExtra.FloatValue;
        }
        if (itemEquipment.EquipmentRarity >= ERarity.Legendary)
        {
            extra += legendaryDmgExtra.FloatValue;
        }
    }
    private void OnBeforeHit(ActorBase attack, ActorBase defend, DamageSource dmg)
    {
        if (attack != Caster) return;
        if (itemEquipment.EquipmentRarity < ERarity.Epic) return;
        if (!defend.Tagger.HasAnyTags(new List<ETag>() { ETag.Boss, ETag.Elite, ETag.MiniBoss })) return;
        //dmg.Value *= (1 + extra);
        dmg.AddModifier(new StatModifier(EStatMod.PercentAdd, extra));
        Debug.Log("[Passive Pirate Necklace] Appied Increase Dmg Except Boss, Elite");
    }
}