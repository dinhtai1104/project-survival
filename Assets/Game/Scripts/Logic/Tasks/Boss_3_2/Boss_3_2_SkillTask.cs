using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss_3_2_SkillTask : SkillTask
{
    public int TrackAnim = 1;
    public string animationSkill;
    public CirclePatternBulletSpawner circlePrefab;

    public Transform shotPos;
    public ValueConfigSearch bulletSize;
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch bulletVelocity;

    public ValueConfigSearch circleVelocity;
    public ValueConfigSearch circleBulletDistanceFromCenter;
    public ValueConfigSearch circleAmountBullet;
    public ValueConfigSearch circleRotateSpeed;

    public ValueConfigSearch maxReflectBullet;
    public string VFX_Shot = "VFX_Boss_3002_Shoot";

    public override async UniTask Begin()
    {
        await base.Begin();
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }
        bulletSize.SetId(Caster.gameObject.name);
        bulletDmg.SetId(Caster.gameObject.name);
        bulletVelocity.SetId(Caster.gameObject.name);
        circleVelocity.SetId(Caster.gameObject.name);
        circleBulletDistanceFromCenter.SetId(Caster.gameObject.name);
        circleAmountBullet.SetId(Caster.gameObject.name);
        circleRotateSpeed.SetId(Caster.gameObject.name);
        maxReflectBullet.SetId(Caster.gameObject.name);

        Caster.AnimationHandler.SetAnimation(TrackAnim, animationSkill, false);
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
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        base.OnStop();
    }

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            IsCompleted = true;
        }
    }


    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                GameObjectSpawner.Instance.Get(VFX_Shot, (t) =>
                {
                    t.GetComponent<Game.Effect.EffectAbstract>().Active(shotPos.position);
                });
                ReleaseBullet();
            }
        }
    }

    protected virtual void ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        var circle = PoolManager.Instance.Spawn(circlePrefab);
        circle.transform.position = shotPos.position;

        circle.SpawnItem(circleAmountBullet.IntValue, bulletSize.FloatValue, OnCreateBullet);
        circle.SetCaster(Caster);

        var rotateSpeed = new Stat(circleRotateSpeed.FloatValue);
        var statSpeed = new Stat(circleVelocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        circle.Rotate.Speed = rotateSpeed;
        circle.Movement.Speed = statSpeed;
        circle.Movement.SetDirection((target.GetMidTransform().position - shotPos.position).normalized);
        circle.Play();
    }

    void OnCreateBullet(BulletSimpleDamageObject bullet)
    {
        var statSpeed = new Stat(circleVelocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);

        bullet.transform.position += bullet.transform.right * circleBulletDistanceFromCenter.FloatValue;
        bullet.SetCaster(Caster);
        bullet.Movement.Speed = statSpeed;
        bullet.Movement.IsActive = false;
        bullet.SetMaxHit(maxReflectBullet.IntValue);
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.FloatValue);
        bullet.Play();
        bullet._hit.onTrigger += OnTrigger;

        void OnTrigger(Collider2D collider, ITarget target)
        {
            bullet.onBeforePlay?.Invoke();
            bullet.Movement.IsActive = true;
            bullet.transform.SetParent(null);
        }
    }
}