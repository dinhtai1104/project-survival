using DG.Tweening;
using Game.GameActor;
using System;
using UnityEngine;
public class SpiderSlowMark 
{
    public SpiderTileNet Current;
    public ActorBase Target;
    public float LastTrigger = 0;
}

public class SpiderTileNet : TileInstanceBase
{
    public ValueConfigSearch Drag,SpeedMultiplier,Delay;
    public static SpiderSlowMark SpiderSlowMark = new SpiderSlowMark()
    {
    };

    float activeTime = 0;
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase>(EventKey.OnCharacterJump, OnCharacterJump);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Messenger.RemoveListener<ActorBase>(EventKey.OnCharacterJump, OnCharacterJump);
    }

    private void OnCharacterJump(ActorBase arg1)
    {
        activeTime = Time.time;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var target = collision.transform.GetComponentInParent<ActorBase>();
        if (target == null) return;

        if (!target.Tagger.HasAnyTags(ETag.Player))
        {
            return;
        }
        if (SpiderSlowMark.Current == this)
        {
            target.MoveHandler.BonusDrag = 0;
            target.Stats.RemoveModifiersFromSource(SpiderSlowMark);
            SpiderSlowMark.Current = null;
            SpiderSlowMark.Target = null;
            SpiderSlowMark.LastTrigger = Time.time;

        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.transform.GetComponentInParent<ActorBase>();
        if (target == null) return;

        if (!target.Tagger.HasAnyTags(ETag.Player))
        {
            return;
        }

        StopMovement(target);
    }

    private void StopMovement(ActorBase target)
    {
        //if (SpiderSlowMark.Current==this &&Time.time - SpiderSlowMark.LastTrigger < 0.5f) return;
        if (Time.time - SpiderSlowMark.LastTrigger < Delay.FloatValue && Time.time-target.MoveHandler.lastJump < Delay.FloatValue) return;
        target.Stats.RemoveModifiersFromSource(SpiderSlowMark);
        SpiderSlowMark.Current = this;
        if (SpiderSlowMark.Current == null)
        {
            target.Stats.AddModifier(StatKey.SpeedMove, new StatModifier(EStatMod.PercentMul, SpeedMultiplier.FloatValue), SpiderSlowMark);
        }
        SpiderSlowMark.Current = this;
        SpiderSlowMark.Target = target;
        target.MoveHandler.BonusDrag = Drag.FloatValue;
        target.MoveHandler.ResetJump();

        SpiderSlowMark.LastTrigger = Time.time;
    }
}