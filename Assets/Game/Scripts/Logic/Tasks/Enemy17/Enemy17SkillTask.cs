using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy17SkillTask : SkillTask
{
    public Enemy17CircleBall circleBallPrefab;
    public string skillAnimation;
    public ParticleSystem muzzlePS;
    public Transform shotPos;

    public ValueConfigSearch bullet_Size;
    public ValueConfigSearch bullet_Dmg;
    public ValueConfigSearch circle_MoveSpeed;
    public ValueConfigSearch circle_RotateSpeed;
    public ValueConfigSearch circle_Radius;
    public ValueConfigSearch circle_Amount;

    public override async UniTask Begin()
    {
        await base.Begin();
        var target = Caster.FindClosetTarget() as ActorBase;
        if (target == null)
        {
            IsCompleted = true;
            return;
        }
        Caster.AnimationHandler.SetAnimation(1,skillAnimation, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
    }


    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        return base.End();
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        muzzlePS.Play();
        SpawnCircleBullet();
        IsCompleted = true;
    }

    private void SpawnCircleBullet()
    {
        var target = Caster.FindClosetTarget() as ActorBase;
        if (target == null) return;
        var dir = target.GetMidTransform().position - Caster.GetMidTransform().position;

        var circle = PoolManager.Instance.Spawn(circleBallPrefab);
        circle.transform.position = shotPos.position;

        circle.SetCaster(Caster);
        circle.SpawnItem(circle_Amount.SetId(Caster.gameObject.name).IntValue, 1f, OnCreateBullet);
        var speedVelocity = new Stat(circle_MoveSpeed.SetId(Caster.gameObject.name).FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(speedVelocity) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);

        circle.Movement.Speed = speedVelocity;
        circle.Rotate.Speed = new Stat(circle_RotateSpeed.SetId(Caster.gameObject.name).FloatValue);

        circle.Movement.SetDirection(dir.normalized);
        circle.Play();
    }

    private void OnCreateBullet(CharacterObjectBase bullet)
    {
        (bullet as BulletSimpleDamageObject).DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bullet_Dmg.SetId(Caster.gameObject.name).FloatValue);
        (bullet as BulletSimpleDamageObject).SetCaster(Caster);
        (bullet as BulletSimpleDamageObject).SetMaxHit(1);
        (bullet as BulletSimpleDamageObject).Play();

        bullet.transform.localScale = Vector3.one * bullet_Size.SetId(Caster.gameObject.name).FloatValue;
        bullet.transform.position += bullet.transform.right * circle_Radius.SetId(Caster.gameObject.name).FloatValue;
    }
}