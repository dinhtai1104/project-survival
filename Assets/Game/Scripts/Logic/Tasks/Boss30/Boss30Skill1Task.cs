using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss30Skill1Task : SkillTask
{
    [SerializeField] private string Muzzle = "VFX_Boss30_Skill1_Muzzle";

    [SerializeField] private ValueConfigSearch timeBtwBallThrow;
    [SerializeField] private ValueConfigSearch amountBall;
    [SerializeField] private ValueConfigSearch velocityBall;
    [SerializeField] private ValueConfigSearch radiusExplosion;
    [SerializeField] private ValueConfigSearch dmgExplosion;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform beltPos;

    [SerializeField] private string skill1_First;
    [SerializeField] private string skill1_Shot;

    public AudioClip shootSFX;

    private int CurrentBullet = 0;
    public override async UniTask Begin()
    {
        await base.Begin();
        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            IsCompleted = true;
            return;
        }

        Caster.AnimationHandler.SetAnimation(skill1_First, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnEventComplete;
        CurrentBullet = 0;
        //Timing.RunCoroutine(_Shoot());
    }
    public override async UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnEventComplete; 
        await base.End();
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "attack_tracking")
        {
            _ShootOne();
        }
    }

    private void OnEventComplete(TrackEntry trackEntry)
    {
        if (CurrentBullet >= amountBall.IntValue)
        {
            if (trackEntry.Animation.Name == "idle")
            {
                IsCompleted = true;
                return;
            }
            Caster.AnimationHandler.SetIdle();
            return;
        }
        if (trackEntry.Animation.Name == skill1_First || trackEntry.Animation.Name == skill1_Shot)
        {
            Caster.AnimationHandler.SetAnimation(skill1_Shot, false);
        }
    }

    private void _ShootOne()
    {
        CurrentBullet++;

        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            IsCompleted = true;
            return;
        }

        Game.Pool.GameObjectSpawner.Instance.Get(Muzzle, t =>
        {
            t.GetComponent<Game.Effect.EffectAbstract>().Active(beltPos.position);
        });

        var ball = PoolManager.Instance.Spawn(ballPrefab).GetComponent<BulletSimpleDamageObject>();
        ball.transform.position = beltPos.position;
        var dir = target.GetMidTransform().position - beltPos.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        ball.transform.eulerAngles = new Vector3(0, 0, angle);
        var velocity = new Stat(velocityBall.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(velocity) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        ball.Movement.Speed = velocity;
        ball.SetCaster(Caster);
        ball.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg));
        ball.GetComponent<IRadiusRequire>().Radius = new Stat(radiusExplosion.FloatValue);
        ball.SetMaxHit(1);
        ball.Play();
    }

    private IEnumerator<float> _Shoot()
    {
        for (int i = 0; i < amountBall.IntValue - 1; i++)
        {
            _ShootOne();
            yield return Timing.WaitForSeconds(timeBtwBallThrow.FloatValue);
        }
        IsCompleted = true;
    }
}