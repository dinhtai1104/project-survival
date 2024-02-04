using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy7001Skill2Task : SkillTask
{
    public string _animation;
    public string _vfx = "";
    public BulletSimpleDamageObject bigStonePrefab;
    public BulletSimpleDamageObject smallStonePrefab;
    public Transform pos;

    [Header("Big Stone")]
    public ValueConfigSearch bigStone_Size;
    public ValueConfigSearch bigStone_Dmg;
    public ValueConfigSearch bigStone_Velocity;
    
    [Header("Small Stone")]
    public ValueConfigSearch smallStone_Size;
    public ValueConfigSearch smallStone_Dmg;
    public ValueConfigSearch smallStone_Velocity;
    public ValueConfigSearch smallStone_NumberEachSide;
    public ValueConfigSearch smallStone_AngleOffset;

    public override async UniTask Begin()
    {
        await base.Begin();
        if (GameController.Instance.GetMainActor() == null)
        {
            IsCompleted = true;
            return;
        }
        Caster.AnimationHandler.SetAnimation(0, _animation, false);
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

    protected virtual void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            IsCompleted = true;
        }
    }

    protected virtual void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            if (e.Data.Name == "attack_tracking")
            {
                if (!string.IsNullOrEmpty(_vfx))
                {
                    GameObjectSpawner.Instance.Get(_vfx, (t) =>
                    {
                        t.GetComponent<Game.Effect.EffectAbstract>().Active(pos.position, Caster.GetLookDirection());
                    });
                }
                ReleaseBullet();
            }
        }
    }

    protected virtual BulletSimpleDamageObject ReleaseBigBullet( float angle)
    {
        var bullet = PoolManager.Instance.Spawn(bigStonePrefab);
        bullet.transform.position = pos.position;

        bullet.transform.eulerAngles = Vector3.forward * angle;
        bullet.transform.localScale = Vector3.one * bigStone_Size.FloatValue;

        var statSpeed = new Stat(bigStone_Velocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = statSpeed;
        bullet.SetCaster(Caster);
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bigStone_Dmg.SetId(Caster.gameObject.name).FloatValue);
        bullet.SetMaxHit(1);
        bullet.SetMaxHitToTarget(1);
        bullet.Play();
        return bullet;
    }

    protected virtual void ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            target = GameController.Instance.GetMainActor();
        }
        if (target == null) return;
        var angle = GameUtility.GameUtility.GetAngle(pos, target.GetMidTransform());

        // Release BigBullet
        ReleaseBigBullet(angle);
        // Release SmallBullet
        ReleaseSmallBullet(angle);
    }

    private void ReleaseSmallBullet(float angle)
    {
        float angleCal = smallStone_AngleOffset.FloatValue;

        for (int i = 0; i < smallStone_NumberEachSide.IntValue; i++)
        {
            ReleaseSmallBulletInSide(angle + angleCal);
            ReleaseSmallBulletInSide(angle + -angleCal);
            angle += smallStone_AngleOffset.FloatValue;
        }
    }

    private void ReleaseSmallBulletInSide(float angle)
    {
        var bullet = PoolManager.Instance.Spawn(smallStonePrefab);
        bullet.transform.position = pos.position;

        bullet.transform.eulerAngles = Vector3.forward * angle;
        bullet.transform.localScale = Vector3.one * smallStone_Size.FloatValue;

        var statSpeed = new Stat(smallStone_Velocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = statSpeed;
        bullet.SetCaster(Caster);
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * smallStone_Dmg.SetId(Caster.gameObject.name).FloatValue);
        bullet.SetMaxHit(1);
        bullet.SetMaxHitToTarget(1);
        bullet.Play();
    }
}