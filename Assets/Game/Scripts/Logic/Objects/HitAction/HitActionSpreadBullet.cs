using DG.Tweening;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class HitActionSpreadBullet : MonoBehaviour, IHitTriggerAction, IStateEnterCallback
{
    private ActorBase Caster;
    public BulletSimpleDamageObject bulletPrefab;
    public ValueConfigSearch numberBullet;
    public ValueConfigSearch dmgBullet;
    public ValueConfigSearch sizeBullet;
    public ValueConfigSearch velocityBullet;

    
    public void Action(Collider2D collider)
    {
        var caster = GetComponent<CharacterObjectBase>();
        numberBullet = numberBullet.SetId(caster.Caster.gameObject.name);
        dmgBullet = dmgBullet.SetId(caster.Caster.gameObject.name);
        sizeBullet = sizeBullet.SetId(caster.Caster.gameObject.name);
        velocityBullet = velocityBullet.SetId(caster.Caster.gameObject.name);

        float angle = 0;
        float angleIncre = 360f / numberBullet.IntValue;
        for (int i = 0; i < numberBullet.IntValue; i++)
        {
            ReleaseBullet(angle, caster.Caster);
            angle += angleIncre;
        }
    }

    public void Action()
    {
        numberBullet = numberBullet.SetId(Caster.gameObject.name);
        dmgBullet = dmgBullet.SetId(Caster.gameObject.name);
        sizeBullet = sizeBullet.SetId(Caster.gameObject.name);
        velocityBullet = velocityBullet.SetId(Caster.gameObject.name);

        float angle = 0;
        float angleIncre = 360f / numberBullet.IntValue;
        for (int i = 0; i < numberBullet.IntValue; i++)
        {
            ReleaseBullet(angle, Caster);
            angle += angleIncre;
        }
    }

    public void SetActor(ActorBase actor)
    {
        Caster = actor;
    }

    private void ReleaseBullet(float angle, Game.GameActor.ActorBase Caster)
    {
        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bullet.transform.position += bullet.transform.right * 1f;
        bullet.transform.localScale = Vector3.one * sizeBullet.FloatValue;
        bullet.SetCaster(Caster);
        bullet.DmgStat = new Stat(Caster.GetStatValue(StatKey.Dmg));
        bullet.SetMaxHit(1);
        bullet.SetMaxHitToTarget(1);
        bullet.Movement.SetMove(bullet.transform.right);
        var speed = new Stat(velocityBullet.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(speed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);

        bullet.Movement.Speed = speed;
        bullet.Play();
    }
}
