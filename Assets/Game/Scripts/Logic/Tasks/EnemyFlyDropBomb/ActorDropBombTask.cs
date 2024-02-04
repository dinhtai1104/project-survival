using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorDropBombTask : SkillTask
{
    public string _animation;
    [SerializeField] private Transform dropPos;
    [SerializeField] private ValueConfigSearch velocityBullet;
    [SerializeField] private ValueConfigSearch explosionRadius;
    [SerializeField] private ValueConfigSearch dmgExplosion;

    [SerializeField] private BombObject bulletBulletPrefab;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.SetAnimation(1,_animation, false);
        Caster.AnimationHandler.onEventTracking += OnTrackingEvent;
        Caster.AnimationHandler.onCompleteTracking += OnTrackingComplete;
    }
    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnTrackingEvent;
        Caster.AnimationHandler.onCompleteTracking -= OnTrackingComplete;
        return base.End();
    }

    private void OnTrackingComplete(TrackEntry trackEntry)
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
    private void OnTrackingEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name != _animation) { return; }
        if (e.Data.Name == "attack_tracking")
        {
            var bullet = PoolManager.Instance.Spawn(bulletBulletPrefab);
            bullet.transform.position = dropPos.position;
            bullet.transform.right = Vector2.down;
            var velocityStat = new Stat(velocityBullet.SetId(Caster.gameObject.name).FloatValue);

            var listModi = new List<ModifierSource>() { new ModifierSource(velocityStat) };
            Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
            bullet.Movement.Speed = (velocityStat);
            bullet.SetCaster(Caster);
            bullet.SetMaxHit(1);
            bullet.DmgStat = new Stat(dmgExplosion.SetId(Caster.gameObject.name).FloatValue * Caster.Stats.GetValue(StatKey.Dmg));
            bullet.GetComponent<IRadiusRequire>().Radius = new Stat(explosionRadius.SetId(Caster.gameObject.name).FloatValue);
            bullet.Play();
        }
    }
}