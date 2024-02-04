using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using Spine;
using System.Collections.Generic;
using UnityEngine;

public class SkillTaskShotBullet : SkillTask
{
    public string animationSkill = "attack/combo_1";
    public string VFX_Name = "";
    public BulletSimpleDamageObject bulletPrefab;
    public ValueConfigSearch bulletSize;
    public ValueConfigSearch bulletVelocity;
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch bulletReflect;


    public override async UniTask Begin()
    {
        await base.Begin();
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }
        bulletSize = bulletSize.SetId(Caster.gameObject.name);
        bulletVelocity = bulletVelocity.SetId(Caster.gameObject.name);
        bulletDmg = bulletDmg.SetId(Caster.gameObject.name);
        bulletReflect = bulletReflect.SetId(Caster.gameObject.name);

        Caster.AnimationHandler.SetAnimation(1,animationSkill, false);
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
        if (trackEntry.Animation.Name == animationSkill)
        {
            IsCompleted = true;
        }
    }

    protected virtual void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                if (!string.IsNullOrEmpty(VFX_Name)) {
                    GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
                    {
                        t.GetComponent<Game.Effect.EffectAbstract>().Active(Caster.WeaponHandler.DefaultAttackPoint.position);
                    });
                }
                ReleaseBullet();
            }
        }
    }

    protected virtual BulletSimpleDamageObject ReleaseBullet(float angle)
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = Caster.GetMidTransform().position;
        if (Caster.WeaponHandler.DefaultAttackPoint != null)
        {
            bullet.transform.position = Caster.WeaponHandler.DefaultAttackPoint.position;
        }
        bullet.transform.eulerAngles = Vector3.forward * angle;
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
        return bullet;
    }

    public float GetAngleToTarget()
    {
        var target = Caster.FindClosetTarget();
        if (target == null) return 0;
        var angle = GameUtility.GameUtility.GetAngle(Caster, target);

        return angle;
    }
    protected virtual void ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        var angle = GameUtility.GameUtility.GetAngle(Caster, target);

        ReleaseBullet(angle);
    }
}