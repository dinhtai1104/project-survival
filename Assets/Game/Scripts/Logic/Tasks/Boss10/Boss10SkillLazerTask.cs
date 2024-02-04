using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using MoreMountains.Feedbacks;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
[RequireComponent(typeof(AudioSource))]
public class Boss10SkillLazerTask : SkillTask
{
    public string VFX_Charge = "VFX_Boss10_Skill3_Charge";
    public string _animationReleaseLazer;
    public string _animCharge;
    public string _animEnd;
    public ValueConfigSearch _lazerDmg;
    public ValueConfigSearch _lazerMoveSpeed;
    public ValueConfigSearch _lazerDuration;
    public ValueConfigSearch _lazerSize;
    public ValueConfigSearch _delayBeforeRealShot;

    public MMF_Player feedback;
    [SerializeField] private LazerObject _lazerObject;
    [SerializeField] private Transform posLazer;
    [SerializeField] private LineSimpleControl lazer_ProjectilePrefab;
    private ITarget target;

    private LazerObject lazer;
    private Game.Effect.EffectAbstract _effectCharge;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip _lazerStart;
    [SerializeField] private AudioClip _lazerLoop;
    [SerializeField] private AudioClip _lazerEnd;

    private LineSimpleControl lazer_ProjectileIns;
    private Quaternion endRotate;

    private Vector3 targetLaserPos;

    private void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override async UniTask Begin()
    {
        feedback.StopFeedbacks();
        await base.Begin();
        var target = Caster.Sensor.CurrentTarget;
        if (target == null)
        {
            IsCompleted = true;
            return;
        }
        Caster.MoveHandler.ClearBoostVelocity();
        Caster.AnimationHandler.SetAnimation(_animCharge, false);
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
        Caster.AnimationHandler.onEventTracking += EventTracking;

        targetLaserPos = Caster.Sensor.CurrentTarget.GetMidTransform().position;

        var direction = targetLaserPos - posLazer.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        lazer_ProjectileIns = PoolManager.Instance.Spawn(lazer_ProjectilePrefab);
        lazer_ProjectileIns.transform.position = posLazer.position;
        lazer_ProjectileIns.transform.rotation = targetRotation;

        lazer_ProjectileIns.SetPos(1, targetLaserPos);
        lazer_ProjectileIns.GetComponent<LazerTracking>().SetRotation(new Stat(_lazerMoveSpeed.FloatValue));
        lazer_ProjectileIns.GetComponent<LazerTracking>().SetTarget(Caster.Sensor.CurrentTarget.GetMidTransform());

        Sound.Controller.Instance.Play(audioSource, _lazerStart);

        Game.Pool.GameObjectSpawner.Instance.Get(VFX_Charge, t =>
        {
            _effectCharge = t.GetComponent<Game.Effect.EffectAbstract>();
            _effectCharge.Active(posLazer);
        });
    }
    public override UniTask End()
    {
        feedback.StopFeedbacks();
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        Caster.AnimationHandler.onEventTracking -= EventTracking;
        return base.End();
    }
    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _animCharge)
        {
            Caster.AnimationHandler.SetAnimation(_animationReleaseLazer, true);
            endRotate = lazer_ProjectileIns.transform.rotation;
            PoolManager.Instance.Despawn(lazer_ProjectileIns.gameObject);
        }
    }

    private void EventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animCharge)
        {
            if (e.Data.Name == "attack_tracking")
            {
                _effectCharge.gameObject.SetActive(false);

                Sound.Controller.Instance.Play(audioSource, _lazerLoop, true);
                Timing.RunCoroutine(StartSkill(), gameObject);
            }
        }
    }

    public override void OnStop()
    {
        base.OnStop();
        End();
        audioSource.Stop();
        if (lazer != null)
        {
            PoolManager.Instance.Despawn(lazer.gameObject);
        }
        if (lazer_ProjectileIns != null)
        {
            endRotate = lazer_ProjectileIns.transform.rotation;
            PoolManager.Instance.Despawn(lazer_ProjectileIns.gameObject);
        }
        Timing.KillCoroutines(gameObject);
    }

    private IEnumerator<float> StartSkill()
    {
        yield return Timing.WaitForSeconds(_delayBeforeRealShot.FloatValue);
        ReleaseLazer();
        feedback.PlayFeedbacks();
    }

    private void ReleaseLazer()
    {
        Debug.Log("Release Lazer Skill 3");
        lazer = PoolManager.Instance.Spawn(_lazerObject);
        lazer.transform.position = posLazer.position;
        var target = Caster.Sensor.CurrentTarget;
        if (target == null)
        {
            IsCompleted = true;
            Caster.AnimationHandler.SetAnimation(_animEnd, true);
            return;
        }
        var directon = target.GetMidTransform().position - posLazer.position;
        var angle = Mathf.Atan2(directon.y, directon.x) * Mathf.Rad2Deg;
        var dmgStat = new Stat(_lazerDmg.FloatValue * Caster.Stats.GetValue(StatKey.Dmg));
        var rotateStat = new Stat(_lazerMoveSpeed.FloatValue);
        var duration = _lazerDuration.FloatValue;

        lazer.SetCaster(Caster);
        lazer.SetMaxHitToTarget(99999999);
        lazer.Play(angle, dmgStat, duration);
        lazer.transform.rotation = endRotate;
        //lazer.GetComponent<LazerTracking>().SetStartPos(target.GetPosition());
        lazer.GetComponent<LazerTracking>().SetTarget(target.GetMidTransform());
        lazer.GetComponent<LazerTracking>().SetRotation(rotateStat);
        Timing.RunCoroutine(_WaitForCompleteSkill(duration), gameObject);
    }

    private IEnumerator<float> _WaitForCompleteSkill(float dur)
    {
        yield return Timing.WaitForSeconds(dur);
        OnCompleteTask();
    }

    private void OnCompleteTask()
    {
        IsCompleted = true;
        Caster.AnimationHandler.SetAnimation(_animEnd, true);
        Timing.KillCoroutines(gameObject);

        Sound.Controller.Instance.Play(audioSource, _lazerEnd);
    }
}