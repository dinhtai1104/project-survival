using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using Spine;
using System.Collections.Generic;
using com.mec;
using System;

public class LazerAttackTask : SkillTask
{
    public string animationSkill = "attack/combo_1";
    public string idleSkill = "attack/combo_1";
    public string VFX_Name = "";

    public LineSimpleControl lazerPredictPrefab;
    public LazerObject lazerPrefab;

    private LineSimpleControl predictObj;
    private LazerObject lazerObj;

    public ValueConfigSearch lazerSize;
    public ValueConfigSearch lazerDmg;
    public ValueConfigSearch lazerAimDuration;
    public ValueConfigSearch lazerDuration;
    public ValueConfigSearch lazerDelay;
    public ValueConfigSearch lazerAttackRate;
    public ValueConfigSearch lazerTrackSpeed;

    public Transform pos;
    public UnityEvent eventShoot;



    public override async UniTask Begin()
    {
        await base.Begin();
        if (GameController.Instance.GetMainActor() == null)
        {
            IsCompleted = true;
            return;
        }
        Caster.AnimationHandler.SetAnimation(0, animationSkill, false);
        Caster.AnimationHandler.AddAnimation(0, idleSkill, true);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
    }

    public override UniTask End()
    {
        Timing.KillCoroutines(gameObject);
        if (predictObj != null)
        {
            PoolManager.Instance.Despawn(predictObj.gameObject);
        }
        if (lazerObj != null)
        {
            PoolManager.Instance.Despawn(lazerObj.gameObject);
        }
        predictObj = null;
        lazerObj = null;
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        return base.End();
    }
    public override void OnStop()
    {
        Timing.KillCoroutines(gameObject);
        if (predictObj != null)
        {
            PoolManager.Instance.Despawn(predictObj.gameObject);
        }
        if (lazerObj != null)
        {
            PoolManager.Instance.Despawn(lazerObj.gameObject);
        }
        predictObj = null;
        lazerObj = null;
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        base.OnStop();
    }

    protected virtual void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
        }
    }

    protected virtual void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                if (!string.IsNullOrEmpty(VFX_Name))
                {
                    GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
                    {
                        t.GetComponent<Game.Effect.EffectAbstract>().Active(pos.position);
                    });
                }
                eventShoot?.Invoke();
                ReleaseLazer();
            }
        }
    }

    protected virtual void ReleaseLazer(float angle)
    {
        var target = Caster.FindClosetTarget();
        var laz = PoolManager.Instance.Spawn(lazerPrefab);
        laz.transform.position = pos.position;
        laz.SetCaster(Caster);
        laz.SetMaxHit(-1);
        laz.DmgStat = new Stat(Caster.Stats.GetStat(StatKey.Dmg).Value * lazerDmg.FloatValue);
        laz.transform.localScale = Vector3.one * lazerSize.FloatValue;
        laz.SetMaxHitToTarget(99999);
        //laz.GetComponent<LineSimpleControl>().SetPos(0, pos.position);
        //laz.GetComponent<LineSimpleControl>().SetPos(1, target.GetMidTransform().position);
        laz._hit.SetIntervalTime(lazerAttackRate.FloatValue);
        laz.Play(angle, laz.DmgStat, lazerDuration.FloatValue);
        lazerObj = laz;
    }

    public float GetAngleToTarget()
    {
        var target = Caster.FindClosetTarget();
        if (target == null) return 0;
        var angle = GameUtility.GameUtility.GetAngle(Caster, target);

        return angle;
    }
    protected virtual void ReleaseLazer()
    {
        var target = Caster.FindClosetTarget();

        if (target == null)
        {
            IsCompleted = true;
            return;
        }
        // Create Aim
        Timing.RunCoroutine(_ReleaseIE(), gameObject);
        // Create Lazer
    }

    private IEnumerator<float> _ReleaseIE()
    {
        var target = Caster.FindClosetTarget();
        var angle = GameUtility.GameUtility.GetAngle(pos, target.GetMidTransform());
        float durationAim = lazerAimDuration.FloatValue;
        predictObj = PoolManager.Instance.Spawn(lazerPredictPrefab);
        predictObj.transform.position = pos.position;
        predictObj.SetPos(0, pos.position);
        var posEnd = pos.position;
        posEnd = target.GetMidTransform().position;
        predictObj.SetPos(1, posEnd);
        var dir = Vector3.zero;
        while (durationAim > 0)
        {
            durationAim -= GameTime.Controller.FixedDeltaTime();
            angle = GameUtility.GameUtility.GetAngle(pos, target.GetMidTransform());
            posEnd = target.GetMidTransform().position;
            dir = posEnd - dir;
            predictObj.SetPos(1, posEnd);

            yield return Timing.WaitForSeconds(GameTime.Controller.FixedDeltaTime());
        }
        predictObj.SetPos(1, posEnd + (posEnd - predictObj.GetPos(0)).normalized * 100);

        yield return Timing.WaitForSeconds(lazerDelay.FloatValue);

        PoolManager.Instance.Despawn(predictObj.gameObject);
        predictObj = null;

        ReleaseLazer(angle);
        Timing.RunCoroutine(_WaitForComplete(), gameObject);
    }

    private IEnumerator<float> _WaitForComplete()
    {
        yield return Timing.WaitForSeconds(lazerDuration.FloatValue);
        IsCompleted = true;
    }
}