using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorShotPoisonTask : SkillTask
{
    public string _animation;
    [SerializeField] ValueConfigSearch velocityBullet;
    [SerializeField] ValueConfigSearch radiusPoison;
    [SerializeField] ValueConfigSearch dmgPerTickPoison;
    [SerializeField] ValueConfigSearch timeIntervalPoison;
    [SerializeField] ValueConfigSearch durationPoison;
    [SerializeField] ValueConfigSearch durationStatusPoison;
    [SerializeField] ValueConfigSearch timeIntervalPoisonStatus;

    [SerializeField] private GameObject poisonBulletPrefab;
    [SerializeField] private GameObject poisonPrefab;

    private Stat velocityBulletStat;
    private float poisonIntervalTime;
    private float poisonRadius;
    private float poisonDuration;
    private Stat poisonStatusDuration;
    private Stat poisonStatusTimeInterval;
    private Stat poisonStatusDmg;

    public LayerMask groundMask;

    public override async UniTask Begin()
    {
        await base.Begin();

        Caster.AnimationHandler.SetAnimation(1,_animation, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
        
        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            IsCompleted = true;
            return;
        }
    }

    private void SpawnBullet()
    {
        poisonRadius = radiusPoison.SetId(Caster.gameObject.name).FloatValue;
        poisonIntervalTime = timeIntervalPoison.SetId(Caster.gameObject.name).FloatValue;
        poisonDuration = durationPoison.SetId(Caster.gameObject.name).FloatValue;
        poisonStatusDuration = new Stat(durationStatusPoison.SetId(Caster.gameObject.name).FloatValue);
        poisonStatusTimeInterval = new Stat(timeIntervalPoisonStatus.SetId(Caster.gameObject.name).FloatValue);
        poisonStatusDmg = new Stat(dmgPerTickPoison.SetId(Caster.gameObject.name).FloatValue);

        Vector3 shootPosition = Caster.GetMidTransform().position;

        var bullet = PoolManager.Instance.Spawn(poisonBulletPrefab).GetComponent<BulletSimpleDamageObject>();
        bullet.transform.position = shootPosition;
        var target = Caster.FindClosetTarget();

        if (target == null)
        {
            IsCompleted = true;
            return;
        }
        var direction = target.GetMidTransform().position - shootPosition;
        bullet.transform.right = direction.normalized;
        bullet.SetCaster(Caster);
        bullet.SetMaxHit(1);
        bullet.DmgStat = Caster.Stats.GetStat(StatKey.Dmg);
        velocityBulletStat = new Stat(velocityBullet.SetId(Caster.gameObject.name).FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(velocityBulletStat) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = (velocityBulletStat);
        bullet.GetComponent<InvokeHitByTrigger>().SetIsFullTimeHit(false);
        bullet.GetComponent<InvokeHitByTrigger>().SetMaxHit(1);
        bullet.GetComponent<InvokeHitByTrigger>().onTrigger += OnTrigger;
        bullet.Play();


        async void OnTrigger(Collider2D collider, ITarget target)
        {
            if (target != null)
            {
                var poisonStatus = await ((ActorBase)target).StatusEngine.AddStatus(Caster, EStatus.Poison, "Enemy");
                if (poisonStatus == null) return;
                ((PoisonStatus)poisonStatus).Init(Caster, ((ActorBase)target));
                ((PoisonStatus)poisonStatus).SetCooldown(poisonStatusTimeInterval.Value);
                ((PoisonStatus)poisonStatus).SetDmgMul(poisonStatusDmg.Value);
                ((PoisonStatus)poisonStatus).SetDuration(poisonStatusDuration.Value);
                return;
            }

            var poison = PoolManager.Instance.Spawn(poisonPrefab);
            poison.transform.position = bullet.transform.position;
            poison.GetComponent<InvokeHitByTrigger>().SetIsFullTimeHit(true);
            poison.GetComponent<InvokeHitByTrigger>().SetIntervalTime(poisonIntervalTime);
            poison.GetComponent<AutoDestroyObject>().SetDuration(poisonDuration);

            PoisonObject poisonObject = poison.GetComponent<PoisonObject>();
            poisonObject.SetCaster(Caster);
            poisonObject.SetDuration(poisonStatusDuration);
            poisonObject.SetInterval(poisonStatusTimeInterval);
            poisonObject.SetDamage(poisonStatusDmg);
            poisonObject.SetSource("Enemy");
            poisonObject.Play();
            poison.transform.localScale = Vector3.one * poisonRadius;

            var hit = Physics2D.Raycast(bullet.transform.position,direction, 1.5f, groundMask);
            //Debug.DrawLine(shootPosition, shootPosition + direction * hit.distance, Color.red, 2);
            Debug.DrawRay(hit.point,hit.normal*2, Color.green, 2);

            poison.transform.up = hit.normal;

        }
    }

    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        return base.End();
    }

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _animation)
        {
            IsCompleted = true;
        }
    }
    public override void OnStop()
    {
        base.OnStop();
        End();
        IsCompleted = true;
    }
    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name != _animation)
        {
            return;
        }
        if (e.Data.Name != "attack_tracking") return;
        SpawnBullet();
    }
}