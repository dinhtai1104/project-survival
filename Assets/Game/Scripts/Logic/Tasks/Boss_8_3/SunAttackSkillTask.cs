using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;

public class SunAttackSkillTask : SkillTask
{
    public CirclePatternBulletSpawner circlePrefab;
    public ValueConfigSearch bulletSize;
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch bulletAmountPerCircular;

    public ValueConfigSearch circleDistance;

    public ValueConfigSearch bulletVelocityCircle;


    public ValueConfigSearch rotateSpeedCircle;

    private int currentCircle = 0;
    public override async UniTask Begin()
    {
        await base.Begin();
        currentCircle = 0;
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }

        bulletSize = bulletSize.SetId(Caster.gameObject.name);
        bulletDmg = bulletDmg.SetId(Caster.gameObject.name);
        bulletAmountPerCircular = bulletAmountPerCircular.SetId(Caster.gameObject.name);
        circleDistance = circleDistance.SetId(Caster.gameObject.name);
        bulletVelocityCircle = bulletVelocityCircle.SetId(Caster.gameObject.name);
        rotateSpeedCircle = rotateSpeedCircle.SetId(Caster.gameObject.name);

        ReleaseBullet();
        IsCompleted = true;
    }

    private void ReleaseBullet()
    {
        var circle = PoolManager.Instance.Spawn(circlePrefab);
        circle.transform.position = transform.position;

        circle.SpawnItem(bulletAmountPerCircular.IntValue, bulletSize.FloatValue, OnCreateBullet);
        circle.SetCaster(Caster);

        var rotateSpeed = new Stat(rotateSpeedCircle.FloatValue);

        circle.Rotate.Speed = rotateSpeed;
        circle.Play();
    }

    void OnCreateBullet(BulletSimpleDamageObject bullet)
    {
        int current = currentCircle - 1;
        var statSpeed = new Stat(bulletVelocityCircle.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.transform.position += bullet.transform.right * circleDistance.FloatValue;
        bullet.SetCaster(Caster);
        bullet.Movement.Speed = statSpeed;
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.FloatValue);
        bullet.Play();
    }
}
