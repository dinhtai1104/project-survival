using Game.GameActor;
using Game.GameActor.Buff;
using UnityEngine;
public class BunkerPassive : WeaponPassive
{
    public ValueConfigSearch maxFireRateAdd;
    public ValueConfigSearch fireRateAddValue;
    public ValueConfigSearch timeIncreaseFireRate;

    public ValueConfigSearch legendary_timeIncreaseFireRate;

    private float timeIncreaseFireRateTarget;
    private bool trackingMoving = false;
    private float time = 0;
    private StatModifier fireRateAdd;
        
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase>(EventKey.ContinueMovement, OnContinueMove);
        Messenger.AddListener<ActorBase>(EventKey.StopMovement, OnStopMove);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase>(EventKey.ContinueMovement, OnContinueMove);
        Messenger.RemoveListener<ActorBase>(EventKey.StopMovement, OnStopMove);
    }

    private void OnStopMove(ActorBase source)
    {
        if (source == Caster)
        {
            trackingMoving = false;
        }
    }

    private void OnContinueMove(ActorBase source)
    {
        if (source == Caster)
        {
            trackingMoving = true;
            time = 0;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!trackingMoving)
        {
            time += Time.deltaTime;
            timeIncreaseFireRateTarget = Rarity >= ERarity.Legendary ? legendary_timeIncreaseFireRate.FloatValue : timeIncreaseFireRate.FloatValue;

            if (time >= timeIncreaseFireRateTarget)
            {
                time = 0;
                AddSpeedAttack();
            }
        }
        else
        {
            time = 0;
            if (fireRateAdd.Value != 0)
            {
                fireRateAdd.Value = 0;
            }
        }
    }

    private void AddSpeedAttack()
    {   
        fireRateAdd.Value += fireRateAddValue.FloatValue;
        if (fireRateAdd.Value > maxFireRateAdd.FloatValue)
        {
            fireRateAdd.Value = maxFireRateAdd.FloatValue;
        }
    }
    public override void Play()
    {
        fireRateAdd = new StatModifier(EStatMod.Flat, 0);
        Caster.Stats.AddModifier(StatKey.FireRate, fireRateAdd, this);
    }
}
