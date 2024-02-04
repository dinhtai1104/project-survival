using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;

public class Enemy20SkillTask : SkillTask
{
    public string animationSkill;
    public CirclePatternBulletSpawner circlePrefab;
    public ValueConfigSearch bulletSize;
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch bulletAmountPerCircular;

    public ValueConfigSearch circleDistance;

    public ValueConfigSearch bulletVelocityCircle1;
    public ValueConfigSearch bulletVelocityCircle2;
    public ValueConfigSearch bulletVelocityCircle3;


    public ValueConfigSearch rotateSpeedCircle1;
    public ValueConfigSearch rotateSpeedCircle2;
    public ValueConfigSearch rotateSpeedCircle3;

    public ValueConfigSearch timeDelayCircle2;
    public ValueConfigSearch timeDelayCircle3;

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
        bulletVelocityCircle1 = bulletVelocityCircle1.SetId(Caster.gameObject.name);
        bulletVelocityCircle2 = bulletVelocityCircle2.SetId(Caster.gameObject.name);
        bulletVelocityCircle3 = bulletVelocityCircle3.SetId(Caster.gameObject.name);
        rotateSpeedCircle1 = rotateSpeedCircle1.SetId(Caster.gameObject.name);
        rotateSpeedCircle2 = rotateSpeedCircle2.SetId(Caster.gameObject.name);
        rotateSpeedCircle3 = rotateSpeedCircle3.SetId(Caster.gameObject.name);
        timeDelayCircle2 = timeDelayCircle2.SetId(Caster.gameObject.name);
        timeDelayCircle3 = timeDelayCircle3.SetId(Caster.gameObject.name);

        Caster.AnimationHandler.SetAnimation(0,animationSkill, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
    }

    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        return base.End();
    }
    public override void OnStop()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        base.OnStop();
    }

  

    private IEnumerator<float> _WaitForSkill(int currentCircle)
    {
        yield return Timing.DeltaTime;
        float time = 0;
        if (currentCircle == 1)
        {
            time = timeDelayCircle2.FloatValue;
        }
        if (currentCircle == 2)
        {
            time = timeDelayCircle3.FloatValue;
        }
        yield return Timing.WaitForSeconds(time);
        Caster.AnimationHandler.SetAnimation(0,animationSkill, false);

        if (currentCircle >= 2)
        {
            yield return Timing.WaitForSeconds(1);
            IsCompleted = true;
        }
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                currentCircle++;
                ReleaseBullet(currentCircle - 1);

                Timing.RunCoroutine(_WaitForSkill(currentCircle));
            }
        }
    }

    private void ReleaseBullet(int currentIndex)
    {
        var circle = PoolManager.Instance.Spawn(circlePrefab);
        circle.transform.position = Caster.GetMidTransform().position;

        circle.SpawnItem(bulletAmountPerCircular.IntValue, bulletSize.FloatValue, OnCreateBullet);
        circle.SetCaster(Caster);

        var rotateSpeed = new Stat(0);
        int current = currentIndex;
        if (current == 0)
        {
            rotateSpeed = new Stat(rotateSpeedCircle1.FloatValue);
        }
        if (current == 1)
        {
            rotateSpeed = new Stat(rotateSpeedCircle2.FloatValue);
        }
        if (current == 2)
        {
            rotateSpeed = new Stat(rotateSpeedCircle3.FloatValue);
        }

        circle.Rotate.Speed = rotateSpeed;
        circle.Play();
    }

    void OnCreateBullet(BulletSimpleDamageObject bullet)
    {
        int current = currentCircle - 1;
        var statSpeed = new Stat(0);
        if (current == 0)
        {
            statSpeed = new Stat(bulletVelocityCircle1.FloatValue);
        }
        if (current == 1)
        {
            statSpeed = new Stat(bulletVelocityCircle2.FloatValue);
        }
        if (current == 2)
        {
            statSpeed = new Stat(bulletVelocityCircle3.FloatValue);
        }

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.transform.position += bullet.transform.right * circleDistance.FloatValue;
        bullet.SetCaster(Caster);
        bullet.Movement.Speed = statSpeed;
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.FloatValue);
        bullet.Play();
    }
}
