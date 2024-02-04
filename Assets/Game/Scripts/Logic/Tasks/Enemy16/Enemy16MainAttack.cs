using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy16MainAttack : SkillTask
{
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch bulletVelocity;
    public ValueConfigSearch bulletSize;
    public ValueConfigSearch chargeTime;

    public LineSimpleControl predictPrefab;
    public BulletSimpleDamageObject bulletPrefab;
    public Transform bulletShotPos;
    public string animationSkill;
    public string animationCharge;

    public ParticleSystem chargePS, muzzlePS;

    private CoroutineHandle chargeHandler;

    bool destroy = false;

    public override async UniTask Begin()
    {
        await base.Begin();
        bulletDmg = bulletDmg.SetId(Caster.gameObject.name);
        bulletVelocity = bulletVelocity.SetId(Caster.gameObject.name);
        bulletSize = bulletSize.SetId(Caster.gameObject.name);
        chargeTime = chargeTime.SetId(Caster.gameObject.name);
        destroy = false;
        chargeHandler = Timing.RunCoroutine(_ChargeTime(), gameObject);
    }
    public override async UniTask End()
    {
        destroy = true;
        await base.End();
    }

    private IEnumerator<float> _ChargeTime()
    {

        chargePS.Play();
        Caster.AnimationHandler.SetAnimation(1,animationCharge, true);
        float time = 0;
        //var predict = PoolManager.Instance.Spawn(predictPrefab);
        //predict.transform.position = Caster.WeaponHandler.DefaultAttackPoint.position;
        var target = Caster.FindClosetTarget();
        Caster.SetFacing(target as ActorBase);
        var bullet = ReleaseBullet();
        while (time < chargeTime.FloatValue)
        {
            bullet.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * bulletSize.FloatValue, time / chargeTime.FloatValue);
            time += Time.deltaTime;
            //predict.SetPos(0, Caster.GetMidTransform().position);
            //predict.SetPos(1, target.GetMidTransform().position);
            if (destroy)
            {
                chargePS.Stop();

                bullet.gameObject.SetActive(false);
                IsCompleted = true;
                yield break;
            }
            yield return Timing.DeltaTime;
        }
        var dir = target.GetMidTransform().position - bulletShotPos.position;
        var angle = GameUtility.GameUtility.GetAngle(dir);
        bullet.transform.eulerAngles = Vector3.forward * angle;
        chargePS.Stop();
        muzzlePS.Play();
        bullet.Play();
        //PoolManager.Instance.Despawn(predict.gameObject);
        IsCompleted = true;
    }

    private BulletSimpleDamageObject ReleaseBullet()
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = bulletShotPos.position;
        bullet.SetCaster(Caster);
        bullet.SetMaxHit(1);
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.FloatValue);

        var speed = new Stat(bulletVelocity.FloatValue);
        var listModi = new List<ModifierSource>() { new ModifierSource(speed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = speed;

        return bullet;
    }
}