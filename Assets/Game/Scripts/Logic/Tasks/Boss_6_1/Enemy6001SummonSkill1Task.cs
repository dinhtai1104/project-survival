using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy6001SummonSkill1Task : SkillTask
{
    public string _animation;
    public string _event;
    public Enemy6001SummonSkill1 enemy6001ObjecPrefab;
    private Enemy6001SummonSkill1 objectCurrent;
    public ElipseObject elipseObject;
    public override async UniTask Begin()
    {
        await base.Begin();
        if (objectCurrent == null)
        {
            objectCurrent = PoolManager.Instance.Spawn(enemy6001ObjecPrefab);
            objectCurrent.elipseObject = elipseObject;
            objectCurrent.SetCaster(Caster);
        }
        if (!objectCurrent.IsCanPlay)
        {
            IsCompleted = true;
            return;
        }


        Caster.AnimationHandler.onEventTracking += AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking += AnimationHandler_onCompleteTracking;
        Caster.AnimationHandler.SetAnimation(_animation, false);
        Caster.onSelfDie += Die;
    }

    private void Die(ActorBase current)
    {
        objectCurrent.Stop();
    }

    private void AnimationHandler_onCompleteTracking(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            IsCompleted = true;
        }
    }

    public override void OnStop()
    {
        base.OnStop();
        Caster.onSelfDie -= Die;
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
    }
    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
        Caster.onSelfDie -= Die;
        return base.End();
    }
    private void AnimationHandler_onEventTracking(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            if (e.Data.Name == _event)
            {
                objectCurrent.Play();
            }
        }
    }
}