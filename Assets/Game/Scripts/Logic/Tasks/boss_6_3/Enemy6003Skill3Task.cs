using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Pool;
using Spine;
using System;
using System.Collections.Generic;
using DG.Tweening;
using com.mec;
using Game.GameActor;
using BehaviorDesigner.Runtime.Tasks;

public class Enemy6003Skill3Task : SkillTask
{
    public string animationSkill_Charge = "attack/combo_1";
    public string animationSkill_Attack = "attack/combo_1";
    public string animationSkill_Idle = "attack/combo_1";
    public string VFX_Name = "";

    private ObjectSpawnAmountCircle<CharacterObjectBase> spawnCircleObject;
    public ObjectSpawnAmountCircle<CharacterObjectBase> spawnCircleObjectPrefab;
    public ValueConfigSearch radiusCircle;
    public ValueConfigSearch numberBulletCircle;
    public ValueConfigSearch rotateCircleSpeed;
    public ValueConfigSearch angleEachShot;
    public ValueConfigSearch numberCastSkill;

    // Bullet
    public BulletSimpleDamageObject bulletPrefab;
    public ValueConfigSearch bulletSize;
    public ValueConfigSearch bulletVelocity;
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch bulletReflect;
    public ValueConfigSearch bulletNumber;
    public ValueConfigSearch bulletChaseLevel;
    public Transform pos;

    public UnityEvent eventShoot;
    private int currentAtr = 0;
    private List<CharacterObjectBase> listGates = new List<CharacterObjectBase>();
    public override async UniTask Begin()
    {
        await base.Begin();
        currentAtr = 0;
        rotated = 0;
        isRotate = false;
        Caster.AnimationHandler.onEventTracking += AnimationHandler_onEventTracking;

        Caster.AnimationHandler.SetAnimation(animationSkill_Charge, false);
        Caster.AnimationHandler.AddAnimation(0, animationSkill_Idle, true);
    }

    private void AnimationHandler_onEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill_Charge)
        {
            if (spawnCircleObject == null)
            {
                spawnCircleObject = PoolManager.Instance.Spawn(spawnCircleObjectPrefab);
                spawnCircleObject.GetComponent<AutoFollowObject>().SetFollow(pos);
                spawnCircleObject.transform.position = pos.position;
                spawnCircleObject.SetCaster(Caster);
                spawnCircleObject.SetRadius(radiusCircle.FloatValue);
                spawnCircleObject.SetPosition(pos.position);

                listGates = spawnCircleObject.SpawnItem(numberBulletCircle.IntValue, 1, OnCreateGate);

                spawnCircleObject.transform.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(Ease.InCirc).OnComplete(() =>
                {
                    spawnCircleObject.Rotate.Speed = new Stat(rotateCircleSpeed.FloatValue);
                    spawnCircleObject.Rotate.Play();
                    isRotate = true;
                });
            }
        }
    }

    private void OnCreateGate(CharacterObjectBase obj)
    {
        obj.transform.SetParent(spawnCircleObject.transform);
    }

    public override void OnStop()
    {
        Timing.KillCoroutines(gameObject);
        if (spawnCircleObject != null)
        {
            spawnCircleObject.transform.DOKill();
            spawnCircleObject.Clear();
            PoolManager.Instance.Despawn(spawnCircleObject.gameObject);
            spawnCircleObject = null;
        }
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        base.OnStop();
    }

    private IEnumerator<float> PrepareAndShot()
    {
        isRotate = false;
        spawnCircleObject.Rotate.PauseRotate();
        foreach (var gate in listGates)
        {
            gate.transform.DOScale(Vector3.one * 2, 0.5f);
        }
        yield return Timing.WaitForSeconds(0.6f);
        Caster.AnimationHandler.SetAnimation(animationSkill_Attack, false);
        Caster.AnimationHandler.AddAnimation(0, animationSkill_Idle, true);
        foreach (var gate in listGates)
        {
            SpawnBullet(gate.transform);
        }
        yield return Timing.WaitForSeconds(0.3f);
        spawnCircleObject.Rotate.ResumeRotate();
        foreach (var gate in listGates)
        {
            gate.transform.DOScale(Vector3.one * 1, 0.5f);
        }
        isRotate = true;
        if (currentAtr < numberCastSkill.IntValue)
        {
        }
        else
        {
            IsCompleted = true;
        }
    }

    private BulletSimpleDamageObject SpawnBullet(Transform transformGate)
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = transformGate.position;

        bullet.transform.rotation = transformGate.rotation;
        bullet.transform.localScale = Vector3.one * bulletSize.FloatValue;

        var statSpeed = new Stat(bulletVelocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = statSpeed;
        bullet.SetCaster(Caster);
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.SetId(Caster.gameObject.name).FloatValue);
        bullet.SetMaxHit(bulletReflect.IntValue > 1 ? bulletReflect.IntValue : 1);
        bullet.SetMaxHitToTarget(1);
        bullet.Play();
        bullet.Movement.TrackTarget(bulletChaseLevel.FloatValue, GameController.Instance.GetMainActor().transform);

        if (!string.IsNullOrEmpty(VFX_Name))
        {
            GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
            {
                t.GetComponent<Game.Effect.EffectAbstract>().Active(transformGate);
            });
        }
        eventShoot?.Invoke();

        return bullet;
    }

    private void PrepareGate()
    {
        if (currentAtr < numberCastSkill.IntValue)
        {
            Caster.AnimationHandler.SetAnimation(0, animationSkill_Idle, true);
        }
        else
        {
            IsCompleted = true;
            return;
        }

        currentAtr++;
        foreach (var gate in listGates)
        {
            gate.transform.DOScale(Vector3.one * 1, 0.2f);
        }

        Timing.RunCoroutine(PrepareAndShot(), gameObject);
    }
    public override UniTask End()
    {
        Timing.KillCoroutines(gameObject);
        if (spawnCircleObject != null)
        {
            spawnCircleObject.transform.DOKill();
            spawnCircleObject.Clear();
            PoolManager.Instance.Despawn(spawnCircleObject.gameObject);
        }
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        return base.End();
    }

    private float rotated = 0;
    private bool isRotate;
    public override void Run()
    {
        base.Run();
        if (!isRotate) { return; }
        rotated += Time.deltaTime * rotateCircleSpeed.FloatValue;
        if (rotated > angleEachShot.FloatValue)
        {
            PrepareGate();
            rotated = 0;
        }
    }
    public override bool HasTask()
    {
        if (GameController.Instance.GetMainActor() == null)
        {
            return false;
        }
        return true;
    }
}