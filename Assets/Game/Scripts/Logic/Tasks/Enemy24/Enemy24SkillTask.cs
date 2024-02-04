using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System.Collections.Generic;
using UnityEngine;

public class Enemy24SkillTask : SkillTask
{
    public string animationSkill;
    public BulletSimpleDamageObject bulletPrefab;
    public ValueConfigSearch bullet_Size;
    public ValueConfigSearch bullet_Dmg;
    public ValueConfigSearch bullet_Velocity;


    public override async UniTask Begin()
    {
        await base.Begin();
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }
        bullet_Size = bullet_Size.SetId(Caster.gameObject.name);
        bullet_Dmg = bullet_Dmg.SetId(Caster.gameObject.name);
        bullet_Velocity = bullet_Velocity.SetId(Caster.gameObject.name);

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
                ReleaseBullet();
            }
        }
    }

    private void ReleaseBullet()
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = Caster.GetMidTransform().position;

        var target = Caster.FindClosetTarget();
        var angle = GameUtility.GameUtility.GetAngle(Caster, target);
        bullet.transform.eulerAngles = Vector3.forward * angle;
        bullet.transform.localScale = Vector3.one * bullet_Size.FloatValue;

        var statSpeed = new Stat(bullet_Velocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi); 
        bullet.Movement.Speed = statSpeed;
        bullet.SetCaster(Caster);
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bullet_Dmg.FloatValue);
        bullet.SetMaxHit(1);
        bullet.Play();
    }
}