using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCirclePatternBullet : MonoBehaviour, IBeforeDestroyObject
{
    private CharacterObjectBase objectBase;
    public CirclePatternBulletSpawner spawner;
    public ValueConfigSearch spawnAmount;
    public ValueConfigSearch size;
    public ValueConfigSearch dmg;
    public ValueConfigSearch velocity;
    public ValueConfigSearch distanceFromCenter;
    private void Awake()
    {
        objectBase = GetComponent<CharacterObjectBase>();
    }

    public void Action(Collider2D collision)
    {
        var s = PoolManager.Instance.Spawn(spawner);
        s.transform.position = transform.position;
        s.SpawnItem(spawnAmount.IntValue, 1, OnCreate);
    }

    private void OnCreate(BulletSimpleDamageObject bullet)
    {
        bullet.transform.localScale = size.Vector2Value;
        bullet.transform.position += bullet.transform.right * distanceFromCenter.FloatValue;
        var statSpeed = new Stat(velocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, objectBase.Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = statSpeed;
        bullet.DmgStat = new Stat(objectBase.Caster.Stats.GetValue(StatKey.Dmg) * dmg.FloatValue);
        bullet.SetMaxHit(1);
        bullet.SetCaster(objectBase.Caster);
        bullet.Play();
    }
}
