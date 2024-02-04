using Game.Effect;
using Game.GameActor;
using Game.Pool;
using System;
using UnityEngine;

public class SpreadBulletImpactHandler : MonoBehaviour, IHitTriggerAction
{
    public BulletSimpleDamageObject bulletPrefab;
    public ValueConfigSearch numberBullet;
    public ValueConfigSearch explodeForce;
    public ValueConfigSearch bulletGravity;
    public ValueConfigSearch dmgBullet;
    public ValueConfigSearch sizeBullet;
    private CharacterObjectBase Base => GetComponent<CharacterObjectBase>();
    private void SpawnCircleBullet()
    {
        numberBullet = numberBullet.SetId(Base.Caster.gameObject.name);
        explodeForce = explodeForce.SetId(Base.Caster.gameObject.name);
        bulletGravity = bulletGravity.SetId(Base.Caster.gameObject.name);
        dmgBullet = dmgBullet.SetId(Base.Caster.gameObject.name);
        sizeBullet = sizeBullet.SetId(Base.Caster.gameObject.name);

        float angle = 0;
        float angleIncr = 360f / numberBullet.IntValue;

        for (int i = 0; i < numberBullet.IntValue; i++)
        {
            SpawnBullet(angle);
            angle += angleIncr;
        }
    }

    private void SpawnBullet(float angle)
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bullet.transform.position += bullet.transform.right * 1f;
        bullet.transform.localScale = Vector3.one * sizeBullet.FloatValue;
        bullet.SetCaster(Base.Caster);
        bullet.DmgStat = new Stat(Base.Caster.GetStatValue(StatKey.Dmg));
        bullet.SetMaxHit(1);
        bullet.SetMaxHitToTarget(1);
        bullet.Movement.Move(new Stat(0), bullet.transform.right * explodeForce.FloatValue);
        bullet.Play();
    }

    public void Action(Collider2D collider)
    {
        var target = GetComponentInParent<ITarget>();
        SpawnCircleBullet();
    }
}