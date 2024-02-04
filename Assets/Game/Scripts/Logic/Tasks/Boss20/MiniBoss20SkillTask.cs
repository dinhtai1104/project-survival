using Cysharp.Threading.Tasks;
using Spine;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class MiniBoss20SkillTask : SkillTask
{
    [SerializeField] private string _animation;
    [SerializeField] private BulletSimpleDamageObject bulletPrefab;
    [SerializeField] private Transform _shotTrans;
    [SerializeField] private ValueConfigSearch sizeBullet;
    [SerializeField] private ValueConfigSearch amountBullet;
    [SerializeField] private ValueConfigSearch speedBullet;
    [SerializeField] private ValueConfigSearch dmgBullet;

    public override async UniTask Begin()
    {
        await base.Begin();
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }

        Caster.AnimationHandler.SetAnimation(1,_animation, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
    }

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            IsCompleted = true;
        }
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (Caster.FindClosetTarget() == null)
        {
            return;
        }
        if (trackEntry.Animation.Name == _animation)
        {
            if (e.Data.Name == "attack_tracking")
            {
                SpawnBullet();
            }
        }
    }

    private void SpawnBullet()
    {
        SpawnItem(amountBullet.IntValue);
    }
    public void SpawnItem(int amount)
    {
        var target = Caster.FindClosetTarget();
        var angle = GameUtility.GameUtility.GetAngle(target, Caster);
        GenObjects(amount, angle);
    }

    private void GenObjects(int number, float angleStart)
    {
        float incre = 360f / number;
        float angleNow = angleStart;

        for (int i = 0; i < number; i++)
        {
            var bl = PoolManager.Instance.Spawn(bulletPrefab);
            bl.transform.position = Caster.GetMidTransform().position;
            bl.transform.localScale = sizeBullet.FloatValue * Vector3.one;
            bl.transform.localEulerAngles = new Vector3(0, 0, angleNow);
            bl.transform.position = _shotTrans.position;
            bl.SetCaster(Caster);
            bl.SetMaxHit(1);
            bl.Movement.Speed = new Stat(speedBullet.FloatValue);
            bl.DmgStat = new Stat(dmgBullet.FloatValue * Caster.Stats.GetValue(StatKey.Dmg));
            bl.Play();
            angleNow += incre;
        }
    }

    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        return base.End();
    }
}