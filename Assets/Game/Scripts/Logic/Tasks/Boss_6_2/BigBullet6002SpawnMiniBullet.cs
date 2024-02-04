using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BigBullet6002SpawnMiniBullet : MonoBehaviour, IUpdate
{
    private CharacterObjectBase Base;
    public BulletSimpleDamageObject bulletPrefab;
    public ValueConfigSearch durationFly;
    public ValueConfigSearch timeSpawnBtw;
    public ValueConfigSearch angleOffset;
    public ValueConfigSearch sizeMiniBullet;
    public ValueConfigSearch velocityMiniBullet;
    public ValueConfigSearch dmgMiniBullet;
    public ValueConfigSearch reflectMiniBullet;

    private AutoDestroyObject autoDestroy;
    private float time = 0;
    public void OnInit()
    {
        Base = GetComponent<CharacterObjectBase>();
        autoDestroy = GetComponent<AutoDestroyObject>();
        autoDestroy.SetDuration(durationFly.FloatValue);
    }

    public void OnUpdate()
    {
        time += Time.deltaTime;
        if (time > timeSpawnBtw.FloatValue)
        {
            time = 0;
            SpawnMiniBullet();
        }
    }

    private void SpawnMiniBullet()
    {
        float angle = GameUtility.GameUtility.GetAngle(transform.right);
        SpawnMiniBullet(angle + angleOffset.FloatValue);
        SpawnMiniBullet(angle - angleOffset.FloatValue);
    }
    private BulletSimpleDamageObject SpawnMiniBullet(float angle)
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = transform.position;

        bullet.transform.eulerAngles = Vector3.forward * angle;
        bullet.transform.localScale = Vector3.one * sizeMiniBullet.FloatValue;

        var statSpeed = new Stat(velocityMiniBullet.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Base.Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = statSpeed;
        bullet.SetCaster(Base.Caster);
        bullet.DmgStat = new Stat(Base.Caster.Stats.GetValue(StatKey.Dmg) * dmgMiniBullet.SetId(Base.Caster.gameObject.name).FloatValue);
        bullet.SetMaxHit(reflectMiniBullet.IntValue > 1 ? reflectMiniBullet.IntValue : 1);
        bullet.SetMaxHitToTarget(1);
        bullet.Play();
        return bullet;
    }
}