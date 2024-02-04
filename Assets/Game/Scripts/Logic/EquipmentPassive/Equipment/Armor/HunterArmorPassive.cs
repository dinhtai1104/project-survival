using Game.GameActor;
using UnityEngine;

public class HunterArmorPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicDmgDecrease;
    public ValueConfigSearch legendaryDmgDecrease;

    private float dmgDecrease;

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
        dmgDecrease = 1;
        if (itemEquipment.EquipmentRarity >= ERarity.Epic)
        {
            dmgDecrease = epicDmgDecrease.FloatValue;
        }
        if (itemEquipment.EquipmentRarity >= ERarity.Legendary)
        {
            dmgDecrease = legendaryDmgDecrease.FloatValue;
        }
    }
    private void OnBeforeHit(ActorBase attack, ActorBase defend, DamageSource dmg)
    {
        if (defend != Caster) return;
        if (itemEquipment.EquipmentRarity < ERarity.Epic) return;
        if (attack.Tagger.HasTag(ETag.EnemyFly))
        {
            Debug.Log("[Passive Hunter Armor] Appied Decrease Dmg:" + dmgDecrease);
            dmg.Value *= (1 - dmgDecrease);
        }
    }
}