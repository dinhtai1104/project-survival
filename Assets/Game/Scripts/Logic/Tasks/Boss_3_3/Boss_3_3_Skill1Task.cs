using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using GameUtility;
using Spine;
using System.Collections.Generic;
using UnityEngine;

public class Boss_3_3_Skill1Task : SkillTask
{
    public string animationSkill;
    public string VFX_Name;
    public BulletSimpleDamageObject bulletPrefab;
    public Transform shotPos;

    public ValueConfigSearch bigBullet_Size;
    public ValueConfigSearch bigBullet_Dmg;
    public ValueConfigSearch bigBullet_Velocity;
    public ValueConfigSearch bigBullet_MaxReflect;

    public override async UniTask Begin()
    {
        await base.Begin();
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }
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
        //if (trackEntry.Animation.Name == animationSkill)
        //{
        //    IsCompleted = true;
        //}
    }


    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                if (!string.IsNullOrEmpty(VFX_Name))
                {
                    GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
                    {
                        t.GetComponent<Game.Effect.EffectAbstract>().Active(Caster.WeaponHandler.DefaultAttackPoint.position);
                    });
                }
                ReleaseBullet();
                IsCompleted = true;
            }
        }
    }

    private void ReleaseBullet()
    {
        
        var target = Caster.FindClosetTarget();
        if (target == null) return;
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);

        bullet.transform.position = shotPos.position;
        bullet.transform.localScale = Vector3.one * bigBullet_Size.FloatValue;
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bigBullet_Dmg.FloatValue);
        var statSpeed = new Stat(bigBullet_Velocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi); bullet.Movement.Speed = statSpeed;
        bullet.SetCaster(Caster);
        bullet.SetMaxHit(bigBullet_MaxReflect.IntValue);

        bullet.Movement.SetDirection(target.GetMidTransform().position - shotPos.position);
        bullet.Play();
    }
}