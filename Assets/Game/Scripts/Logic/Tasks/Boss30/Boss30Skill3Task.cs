using com.mec;
using Cysharp.Threading.Tasks;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss30Skill3Task : SkillTask
{
    [SerializeField] private ValueConfigSearch dmgLazer;
    [SerializeField] private ValueConfigSearch lazerDuration;
    [SerializeField] private ValueConfigSearch lazerSize;
    [SerializeField] private ValueConfigSearch rotateSpeed;
    [SerializeField] private ValueConfigSearch delayBeforeRealShot;
    [SerializeField] private Boss30Skill3CircleLazers lazersObject;
    [SerializeField] private Boss30Skill3LazerPredict lazersPredictPrefab;
    [SerializeField] private Transform beltPos;

    [SerializeField] private string skill_Start;
    [SerializeField] private string skill_Loop;
    [SerializeField] private string skill_End;

    private Boss30Skill3LazerPredict lazerPredict;
    private Quaternion rotateEnd;

    public AudioClip chargeSFX,shootLoopSFX,endSFX;
    public override async UniTask Begin()
    {
        await base.Begin();

        Caster.AnimationHandler.SetAnimation(skill_Start, false);
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
        Caster.AnimationHandler.onEventTracking += OnEventTracking;

        lazerPredict = PoolManager.Instance.Spawn(lazersPredictPrefab);
        lazerPredict.transform.position = beltPos.position;
        lazerPredict.SpawnItem(4, lazerSize.FloatValue, null);
        lazerPredict.Rotate.Speed = new Stat(rotateSpeed.FloatValue);
        lazerPredict.Play();

        Caster.SoundHandler.PlayOneShot(chargeSFX, 1);
    }

    public override async UniTask End()
    {
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        await base.End();
    }
    public override void OnStop()
    {
        if (lazerPredict != null)
        {
            rotateEnd = lazerPredict.transform.rotation;
            lazerPredict.Clear();
            PoolManager.Instance.Despawn(lazerPredict.gameObject);
            lazerPredict = null;
        }
        base.OnStop();
    }

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        var anim = trackEntry.Animation.Name;
        if (anim == skill_Start)
        {
            Caster.AnimationHandler.SetAnimation(skill_Loop, true);
        }
        if (anim == skill_End)
        {
            IsCompleted = true;
        }
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack_tracking")
        {
            if (lazerPredict != null)
            {
                rotateEnd = lazerPredict.transform.rotation;
                lazerPredict.Clear();
                PoolManager.Instance.Despawn(lazerPredict.gameObject);
                lazerPredict = null;
            }

            Timing.RunCoroutine(_SpawnCircleLazer());
        }
    }

    private IEnumerator<float> _SpawnCircleLazer()
    {
        yield return Timing.WaitForSeconds(delayBeforeRealShot.FloatValue);
        Caster.SoundHandler.PlayLoop(shootLoopSFX, 1);

        var obj = PoolManager.Instance.Spawn(lazersObject);
        obj.transform.position = beltPos.position;
        obj.SpawnItem(4, lazerSize.FloatValue, OnSetupLazer);

        obj.GetComponent<AutoDestroyObject>().SetDuration(lazerDuration.FloatValue + 0.2f);
        obj.GetComponent<AutoDestroyObject>().onComplete += OnCompleteSkill;
        obj.Rotate.Speed = new Stat(rotateSpeed.FloatValue);
        obj.transform.rotation = rotateEnd;
        obj.Play();
        void OnCompleteSkill()
        {
            Caster.AnimationHandler.SetAnimation(skill_End, false);
             Caster.SoundHandler.Stop();
        }

        yield return Timing.WaitForSeconds(1f);
    }

    private void OnSetupLazer(LazerObject obj)
    {
        obj.SetCaster(Caster);
        obj.SetMaxHit(-1);
        obj.SetMaxHitToTarget(99999999);
        obj.Play(obj.transform.eulerAngles.z, 
            new Stat(Caster.Stats.GetValue(StatKey.Dmg) * dmgLazer.FloatValue), 
            lazerDuration.FloatValue);
    }
}