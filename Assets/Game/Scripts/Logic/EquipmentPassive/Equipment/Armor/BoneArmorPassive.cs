using Game.GameActor;
using UnityEngine;

public class BoneArmorPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicImmuneTimes;
    public ValueConfigSearch legendaryImmuneTimes;
    private int immuneTimes = 0;
    private int currentImmuneLeft = 0;
    private void OnEnable()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }

    private void OnBeforeHit(ActorBase attack, ActorBase defender, DamageSource damageSource)
    {
        if (defender != Caster) { return; }
        if (itemEquipment.EquipmentRarity < ERarity.Epic) return;
        if (currentImmuneLeft > 0)
        {
            currentImmuneLeft--;
            Debug.Log("[Passive Bone Armor] Appied Immunes, Left: " + currentImmuneLeft);
            damageSource._damageType = EDamage.Missed;
            damageSource._damage.BaseValue = 0;
            damageSource._damage.Value = 0;

        }
    }

    private void OnStageStart(Callback c)
    {
        currentImmuneLeft = immuneTimes;
    }

    public override void Play()
    {
        base.Play();
        if (itemEquipment.EquipmentRarity >= ERarity.Epic)
        {
            immuneTimes = epicImmuneTimes.IntValue;
        }
        if (itemEquipment.EquipmentRarity >= ERarity.Legendary)
        {
            immuneTimes = legendaryImmuneTimes.IntValue;
        }
        currentImmuneLeft = immuneTimes;
    }

}