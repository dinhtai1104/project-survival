using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System.Collections.Generic;
using UnityEngine;

public class Enemy18SkillTask : SkillTask
{
    public string attackAnimation;
    public string idleAnimation;

    public ValueConfigSearch bulletSize;
    public ValueConfigSearch bulletVelocity;
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch angleOffsetBullet;
    public ValueConfigSearch numberOfBulletSide;
    public ValueConfigSearch timeDelayEachShot;
    public ValueConfigSearch numberOfAttack;

    public BulletSimpleDamageObject bulletPrefab;

    private int currentAttackIndex = 0;
    private float angleToTarget;

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
        angleOffsetBullet = angleOffsetBullet.SetId(Caster.gameObject.name);
        numberOfBulletSide = numberOfBulletSide.SetId(Caster.gameObject.name);
        timeDelayEachShot = timeDelayEachShot.SetId(Caster.gameObject.name);
        numberOfAttack = numberOfAttack.SetId(Caster.gameObject.name);

        currentAttackIndex = 0;
        Caster.AnimationHandler.SetAnimation(0,attackAnimation, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;

        angleToTarget = GameUtility.GameUtility.GetAngle(Caster, Caster.FindClosetTarget());
    } 
    public override async UniTask End()
    {
        await base.End();
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
    }

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        var anim = trackEntry.Animation.Name;
        if (anim == attackAnimation)
        {
            if (currentAttackIndex == numberOfAttack.IntValue)
            {
                IsCompleted = true;
            }
            else
            {
                Caster.AnimationHandler.SetAnimation(idleAnimation, true);
            }
        }
    }

    private IEnumerator<float> _WaitForNextAttack()
    {
        yield return Timing.WaitForSeconds(timeDelayEachShot.FloatValue);
        Caster.AnimationHandler.SetAnimation(0, attackAnimation, false);
    }

    public override void OnStop()
    {
        base.OnStop();
        IsCompleted = true;
        Timing.KillCoroutines(gameObject);
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        var anim = trackEntry.Animation.Name;
        var eventTr = e.Data.Name;

        if (anim == attackAnimation)
        {
            if (eventTr == "attack_tracking")
            {
                currentAttackIndex++;
                ReleaseAttack();
                if (currentAttackIndex < 2)
                {
                    Timing.RunCoroutine(_WaitForNextAttack(), gameObject);
                }
            }
        }
    }

    private void ReleaseAttack()
    {
        ShotBullet(angleToTarget);
        float angleUp = angleToTarget;
        float angleDown = angleToTarget;
        for (int i = -numberOfBulletSide.IntValue; i <= numberOfBulletSide.IntValue; i++)
        {
            if (i == 0) continue;
            var angleOffset = i * angleOffsetBullet.FloatValue;
            ShotBullet(angleUp + angleOffset);
            ShotBullet(angleDown + angleOffset);
        }
    }

    private void ShotBullet(float angle)
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.SetCaster(Caster);
        bullet.SetMaxHit(1);
        bullet.transform.position = Caster.GetMidTransform().position;
        bullet.transform.localScale = Vector3.one * bulletSize.FloatValue;
        var statSpeed = new Stat(bulletVelocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = statSpeed;
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.FloatValue);
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        bullet.Play();
    }
}