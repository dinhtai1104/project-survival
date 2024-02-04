using System.Collections.Generic;
using System;
using UnityEngine;
using Game.GameActor;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class Enemy21DeadAction : MonoBehaviour, IStateEnterCallback
{
    public PatternBulletHellBase patternPrefab;
    public BulletSimpleDamageObject bulletPrefab;
    private ActorBase Actor;

    public ValueConfigSearch bulletSize;
    public ValueConfigSearch bulletDmg;
    public ValueConfigSearch bulletVelocity;
    public ValueConfigSearch bulletReflect;
    public async void Action()
    {


        var pattern = PoolManager.Instance.Spawn(patternPrefab);
        pattern.transform.position = Actor.GetMidTransform().position;
        pattern.Prepare(bulletPrefab, Actor);
        await UniTask.Delay(100);
        pattern.SetSizeBullet(bulletSize.SetId(Actor.gameObject.name).FloatValue);
        pattern.SetDmgBullet(new Stat(Actor.Stats.GetValue(StatKey.Dmg) * bulletDmg.SetId(Actor.gameObject.name).FloatValue));
        pattern.SetMaxHit(bulletReflect.SetId(Actor.gameObject.name).IntValue);

        var speed = new Stat(bulletVelocity.SetId(Actor.gameObject.name).FloatValue);
        var listModi = new List<ModifierSource>() { new ModifierSource(speed) };
        Messenger.Broadcast(EventKey.PreFire, Actor, (BulletBase)null, listModi);
        pattern.SetSpeed(speed);
        pattern.Play();

    }

    public void SetActor(ActorBase actor)
    {
        this.Actor = actor;
    }
}