using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy6001SummonSkill2Task : SkillTask
{
    public string _animation;
    public string _event;
    public string _animationIdle;

    public string _enemyMiniId;

    public ValueConfigSearch numberEachSidePenguin;
    public ValueConfigSearch distancePenguin;
    public ValueConfigSearch hpPenguin;
    public ValueConfigSearch dmgPenguin;
    public LayerMask groundLayer;
    public string VFX_Spawn = "";
    public override async UniTask Begin()
    {
        await base.Begin();

        Caster.AnimationHandler.onEventTracking += AnimationHandler_onEventTracking;
        //Caster.AnimationHandler.onCompleteTracking += AnimationHandler_onCompleteTracking;
        Caster.AnimationHandler.SetAnimation(_animation, false);
        Caster.AnimationHandler.AddAnimation(0, _animationIdle, true);
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
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        //Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
    }
    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        //Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
        return base.End();
    }
    private void AnimationHandler_onEventTracking(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            if (e.Data.Name == _event)
            {
                Summon();
            }
        }
    }

    private async void Summon()
    {
        var spawner = GameController.Instance.GetEnemySpawnHandler();
        if (spawner == null)
        {
            IsCompleted = true;
            return;
        }

        var uniTask = new List<UniTask>();

        int numberLeft = 0;
        int numberRight = 0;
        Collider2D[] coll = new Collider2D[1];
        // Left
        for (int i = 1; i <= numberEachSidePenguin.IntValue; i++)
        {
            var pos = Caster.GetPosition() + i * Vector3.left * distancePenguin.FloatValue;
            var posCheckRaycast = Caster.GetMidPos() + i * Vector3.left * distancePenguin.FloatValue;

            if (Physics2D.OverlapCircleNonAlloc(posCheckRaycast, 0.2f, coll, groundLayer) > 0)
            {
                break;
            }
            numberLeft++;
            var task = spawner.SpawnSingle(_enemyMiniId, (int)Caster.GetStatValue(StatKey.Level), pos + Vector3.up * 0.5f, usePortal: true, vfx_spawn: VFX_Spawn)
                .ContinueWith(enemy =>
                {
                });
            uniTask.Add(task);
        }
        // Right
        for (int i = 1; i <= numberEachSidePenguin.IntValue; i++)
        {
            var pos = Caster.GetPosition() + i * Vector3.right * distancePenguin.FloatValue;
            var posCheckRaycast = Caster.GetMidPos() + i * Vector3.right * distancePenguin.FloatValue;

            if (Physics2D.OverlapCircleNonAlloc(posCheckRaycast, 0.2f, coll, groundLayer) > 0)
            {
                break;
            }
            numberRight++;
            var task = spawner.SpawnSingle(_enemyMiniId, (int)Caster.GetStatValue(StatKey.Level), pos + Vector3.up * 0.5f, usePortal: true, vfx_spawn: VFX_Spawn)
                .ContinueWith(enemy =>
                {
                });
            uniTask.Add(task);
        }

        while (numberLeft < numberEachSidePenguin.IntValue)
        {
            // Add to right

            var pos = Caster.GetPosition() + (numberRight + 1) * Vector3.right * distancePenguin.FloatValue;
            var posCheckRaycast = Caster.GetMidPos() + (numberRight + 1) * Vector3.right * distancePenguin.FloatValue;

            if (Physics2D.OverlapCircleNonAlloc(posCheckRaycast, 0.2f, coll, groundLayer) > 0)
            {
                break;
            }
            numberRight++;
            numberLeft++;
            var task = spawner.SpawnSingle(_enemyMiniId, (int)Caster.GetStatValue(StatKey.Level), pos + Vector3.up * 0.5f, usePortal: true, vfx_spawn: VFX_Spawn)
                .ContinueWith(enemy =>
                {
                });
            uniTask.Add(task);
        }

        while (numberRight < numberEachSidePenguin.IntValue)
        {
            // Add to left
            var pos = Caster.GetPosition() + (numberLeft + 1) * Vector3.left * distancePenguin.FloatValue;
            var posCheckRaycast = Caster.GetMidPos() + (numberLeft + 1) * Vector3.left * distancePenguin.FloatValue;

            if (Physics2D.OverlapCircleNonAlloc(posCheckRaycast, 0.2f, coll, groundLayer) > 0)
            {
                break;
            }
            numberRight++;
            numberLeft++;
            var task = spawner.SpawnSingle(_enemyMiniId, (int)Caster.GetStatValue(StatKey.Level), pos + Vector3.up * 0.5f, usePortal: true, vfx_spawn: VFX_Spawn)
                .ContinueWith(enemy =>
                {
                });
            uniTask.Add(task);
        }
        await UniTask.WhenAll(uniTask);
        IsCompleted = true;
    }
}