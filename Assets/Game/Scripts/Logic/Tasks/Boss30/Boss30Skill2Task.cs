using com.mec;
using Cysharp.Threading.Tasks;
using Game.Tasks;
using Sirenix.Serialization;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss30Skill2Task : SkillTask
{
    [SerializeField] private ValueConfigSearch sizeAreaSpawnLotus;
    [SerializeField] private ValueConfigSearch counterTimeLotusExplosion;
    [SerializeField] private ValueConfigSearch numberOfLotus;
    [SerializeField] private ValueConfigSearch dmgOfLotusExplosion;
    [SerializeField] private ValueConfigSearch radiusOfLotusExplosion;
    [SerializeField] private ValueConfigSearch distanceBtwLotus;

    [SerializeField] private GameObject lotusPrefab;
    [SerializeField] private GameObject explodePrefab;
    [SerializeField] private GameObject effectAreaPrefab;
    [SerializeField] private Transform beltPos;

    [SerializeField] private string skill_Start;
    [SerializeField] private string skill_Loop;
    [SerializeField] private string skill_End;

    public AudioClip spawnSFX,castSFX;
    public override async UniTask Begin()
    {
        await base.Begin();

        Caster.AnimationHandler.SetAnimation(skill_Start, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnEventComplete;
    }
    public override async UniTask End()
    {
        Logger.Log("End skill 2");
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnEventComplete;
        await base.End();
    }
    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack_tracking")
        {
            Timing.RunCoroutine(_SpawnLotus());
        }
    }

    private void OnEventComplete(TrackEntry trackEntry)
    {
        Logger.Log("TEST skill 2");
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
    private IEnumerator<float> _SpawnLotus()
    {
        Caster.SoundHandler.PlayOneShot( castSFX, 1);

        var pos = GameUtility.GameUtility
            .GetRandomPositionWithoutOverlapping(Caster.GetMidTransform().position, sizeAreaSpawnLotus.Vector2Value, distanceBtwLotus.FloatValue, numberOfLotus.IntValue);

        for (int i = 0; i < numberOfLotus.IntValue; i++)
        {
            SpawnLotusPerObject(pos[i]);

            yield return Timing.WaitForSeconds(0.1f);
        }

        yield return Timing.WaitForSeconds(counterTimeLotusExplosion.FloatValue);
        Caster.AnimationHandler.SetAnimation(skill_End, false);
    }

    private void SpawnLotusPerObject(Vector3 pos)
    {
        Sound.Controller.Instance.PlayOneShot(spawnSFX, 0.3f);
        GameObject effArea = null;
        var lotus = PoolManager.Instance.Spawn(lotusPrefab);
        lotus.transform.position = pos;

        var autoDestroy = lotus.GetComponent<AutoDestroyObject>();
      
        autoDestroy.SetDuration(counterTimeLotusExplosion.FloatValue);
       

        autoDestroy.onComplete += CounterEndAction;

        lotus.GetComponentInChildren<TaskRunner>().OnComplete += SpawnEffExplode;

        void SpawnEffExplode()
        {
            effArea = PoolManager.Instance.Spawn(effectAreaPrefab);
            effArea.transform.position = pos;
            effArea.transform.localScale = Vector3.one * radiusOfLotusExplosion.FloatValue;

            lotus.GetComponentInChildren<TaskRunner>().OnComplete -= SpawnEffExplode;
        }

        void CounterEndAction()
        {
            var explodeEff = PoolManager.Instance.Spawn(explodePrefab).GetComponent<DamageExplode>();
            explodeEff.transform.position = pos;
            explodeEff.Init(Caster);
            explodeEff.SetSize(radiusOfLotusExplosion.FloatValue);
            explodeEff.SetDmg(dmgOfLotusExplosion.FloatValue * Caster.Stats.GetValue(StatKey.Dmg));
            explodeEff.Explode();

            if (effArea)
            {
                PoolManager.Instance.Despawn(effArea);
            }
        }
    }

    public override void OnStop()
    {
        base.OnStop();
    }
}