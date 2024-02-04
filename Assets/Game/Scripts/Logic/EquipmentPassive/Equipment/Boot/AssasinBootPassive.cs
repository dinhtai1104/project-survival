
using Game.GameActor;
using System;
using UnityEngine;

public class AssasinBootPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicIncreaseSpeedPercentAdd;
    public ValueConfigSearch epicIncreaseSpeedDuration;

    public ValueConfigSearch legendaryIncreaseSpeedFlat;

    private StatModifier statSpeedPercentAddModifier;
    private StatModifier statSpeedFlatModifier;
    private bool canApplyEpic = true;
    private float currentDuration = 0;

    private void OnEnable()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
    }

    private void OnStageStart(Callback cb)
    {
        if (Rarity < ERarity.Epic) return;
        currentDuration = epicIncreaseSpeedDuration.FloatValue;
        canApplyEpic = true;
    }

    public override void Play()
    {
        base.Play();
        canApplyEpic = false;
        if (Rarity >= ERarity.Epic)
        {
            canApplyEpic = true;
            currentDuration = epicIncreaseSpeedDuration.FloatValue;
            statSpeedPercentAddModifier = new StatModifier(EStatMod.PercentAdd, epicIncreaseSpeedPercentAdd.FloatValue);
        }
        if (Rarity >= ERarity.Legendary)
        {
            statSpeedFlatModifier = new StatModifier(EStatMod.Flat, legendaryIncreaseSpeedFlat.FloatValue);
            Caster.Stats.AddModifier(StatKey.SpeedMove, statSpeedFlatModifier, this);
        }
    }
    public override void Remove()
    {
        base.Remove();        
        Caster.Stats.RemoveModifier(StatKey.SpeedMove, statSpeedPercentAddModifier);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (currentDuration <= 0) return;
        if (canApplyEpic)
        {
            canApplyEpic = false;
            Caster.Stats.AddModifier(StatKey.SpeedMove, statSpeedPercentAddModifier, this);
            Debug.Log("[Passive Assasin Boot] Appied SpeedMove");
            return;
        }
        else
        {
            currentDuration -= Time.deltaTime;
            if (currentDuration <= 0)
            {
                Caster.Stats.RemoveModifier(StatKey.SpeedMove, statSpeedPercentAddModifier);
                return;
            }
        }
    }
}