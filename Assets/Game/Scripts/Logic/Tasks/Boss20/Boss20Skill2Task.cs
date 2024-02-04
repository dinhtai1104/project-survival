using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GameActor;
using Game.Level;
using Spine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Boss20Skill2Task : SkillTask
{
    public float Delay = 1f;
    public ValueConfigSearch heightThrow;
    public ValueConfigSearch timeThrow;
    [SerializeField] private string _chargeAnim1;
    [SerializeField] private string _chargeAnim2;
    [SerializeField] private string _animation;
    [SerializeField] private string _pregnantEnemy = "MiniBoss_20";
    [SerializeField] private AnimationCurve throwCurve;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.SetAnimation(0,_chargeAnim1, false);
        Caster.AnimationHandler.AddAnimation(0, _chargeAnim2, true);

        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
    }
    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        return base.End();
    }

    public override void OnStop()
    {
        base.OnStop();
        Clear();
        End();
    }

    private void Clear()
    {
       
    }

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _chargeAnim1)
        {
            Timing.RunCoroutine(_StartSkill());
            Timing.RunCoroutine(_WaitDelayForStartSkill());
        }

        if (trackEntry.Animation.Name == _animation)
        {
            IsCompleted = true;
        }
    }

    private IEnumerator<float> _WaitDelayForStartSkill()
    {
        yield return Timing.WaitForSeconds(Delay);
        Caster.AnimationHandler.ClearTracks();
        Caster.AnimationHandler.SetAnimation(1,_animation, true);
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            if (e.Data.Name == "attack_tracking")
            {
                PlaySkill();
            }
        }
    }

    private IEnumerator<float> _StartSkill()
    {
        yield return Timing.DeltaTime;
       
    }

    private async void PlaySkill()
    {
        var spawner = BattleGameController.Instance.GetEnemySpawnHandler();
        var level = (int)Caster.Stats.GetValue(StatKey.Level);
        var miniBoss = await spawner.SpawnSingle(_pregnantEnemy, level, Caster.GetMidTransform().position, isStartBehaviourNow: false);
        var endPos = heightThrow.FloatValue * Vector3.up + Caster.GetMidTransform().position;

        await miniBoss.transform.DOMove(endPos, timeThrow.FloatValue).AsyncWaitForCompletion();
        miniBoss.StartBehaviours();
    }
}