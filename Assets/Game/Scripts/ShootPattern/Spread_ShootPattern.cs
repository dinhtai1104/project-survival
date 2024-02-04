using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "ShootPattern/Spread_ShootPattern")]
public class Spread_ShootPattern : ShootPatternBase
{
    [Header("[OBSOLATED]"),HideInInspector]
    public int _bulletCount = 3;
    [Header("[OBSOLATED] total bullet per second, 0: instant")]
    protected float _fireRate;
    [Header("spread angle"),HideInInspector]
    public float _spreadAngle;


    public int _loop = 1;
    [Header("delay spacing between LOOP")]
    public float _loopSpacing;

 
    [Header("spread radius")]
    public float _spreadRadius;
    [Header("emitter tilt angle"),Range(-360f,360f)]
    public float _pitch;

    [Header("total bullet per emitter"),Range(1,10)]
    public int _bulletPerEmitter = 1;
    [Header("emitter bullet offset"), Range(-2f, 2f)]
    public float _emitOffset;

    [Header("rotation of this shoot pattern"),Range(-360f,360f)]
    public float _rotation;

    [Header("use custom direction")]
    public bool useDirectFacing=false;
    [Header("direction of this pattern will follow target")]
    public bool facingTarget = true;
    [Header("center on middle bullet")]
    public bool centerOnMiddleBullet = false;


    public ValueConfigSearch bulletSize, totalBulletPerLoop,emitSpacingTime,SpreadAngle,DelayShootEachBullet;
    CancellationTokenSource cancellation;
    protected string actorTag;
    public override void Destroy()
    {
        if (cancellation != null)
        {
            cancellation.Cancel(); ;
            cancellation.Dispose();
            cancellation = null;
        }
    }

    public override async UniTask<ShootPatternBase> SetUp(ActorBase actor)
    {
        Spread_ShootPattern instance = (Spread_ShootPattern)await base.SetUp(actor);
        instance._loopSpacing = _loopSpacing;
        instance._loop = _loop;
        instance._spreadRadius = _spreadRadius;
        instance._pitch = _pitch;
        instance._emitOffset = _emitOffset;
        instance._bulletPerEmitter = _bulletPerEmitter;
        instance.centerOnMiddleBullet = centerOnMiddleBullet;
        instance.useDirectFacing = useDirectFacing;
        instance.facingTarget = facingTarget;

        instance.bulletSize = bulletSize.Clone().SetId(actor.gameObject.name);
        instance.totalBulletPerLoop = totalBulletPerLoop.Clone().SetId(actor.gameObject.name);
        instance.emitSpacingTime = emitSpacingTime.Clone().SetId(actor.gameObject.name);
        instance.SpreadAngle = SpreadAngle.Clone().SetId(actor.gameObject.name);
        instance.DelayShootEachBullet = DelayShootEachBullet.Clone().SetId(actor.gameObject.name);


        instance._rotation = centerOnMiddleBullet ?-instance.SpreadAngle.FloatValue: _rotation;

        instance._fireRate = emitSpacingTime.FloatValue==0?0:1f / emitSpacingTime.FloatValue;
        //
        instance._bulletCount = instance.totalBulletPerLoop.IntValue;
        instance._spreadAngle = instance.SpreadAngle.FloatValue;
        for (int i = 0; i < instance.totalBulletPerLoop.IntValue; i++)
        {
            ProjectileEmitter emitter = new ProjectileEmitter();
            instance.emitters.Add(emitter);
        }
        
        instance.actorTag = actor.tag;

        return instance;

    }
    public override void Init(ActorBase actor)
    {
        cancellation = new CancellationTokenSource();
       
    }

    public override async UniTask Release()
    {
       
    }

    public override async UniTask Trigger(WeaponBase weapon, Transform triggerPos,float bulletVelocity, Transform target, Vector2 facing, ITarget trackingTarget, AssetReferenceGameObject bulletRef, System.Action onShot)
    {
        if (weapon.character.IsDead()) return;
        for (int i = 0; i < _bulletCount; i++)
        {
            if (i >= emitters.Count)
            {
                emitters.Add(new ProjectileEmitter());
            }
        }
        int count = 0;
        Vector2 triggerPosition = (Vector2)triggerPos.position;
        for (int l = 0; l < _loop; l++)
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                ProjectileEmitter emitter = emitters[i];
                //emitter.direction = useDirectFacing ? facing : RotationToVector(AngleBetweenVectors(triggerPosition, triggerPosition + facing) + _rotation + i * _spreadAngle + _pitch);

                emitter.direction = useDirectFacing ? facing : RotationToVector(AngleBetweenVectors(triggerPosition, triggerPosition + (facingTarget?facing:Vector2.zero) ) + _rotation + i * _spreadAngle + _pitch);
                //Logger.Log(i + " >>> " + emitter.direction +" =>" +useDirectFacing+" "+ facing +" x " + RotationToVector(AngleBetweenVectors(triggerPosition, triggerPosition) + _rotation + i * _spreadAngle + _pitch));
                //Logger.Log(i + " >>>>>>> " + emitter.direction +" =>" +useDirectFacing+" "+ facing +" x " + RotationToVector(AngleBetweenVectors(triggerPosition, triggerPosition + facing) + _rotation + i * _spreadAngle + _pitch));
                emitter.position = triggerPosition + RotationToVector(_rotation + i * _spreadAngle).normalized * _spreadRadius;
                emitter.power = facing.magnitude == 0 ? 1 : facing.magnitude;


                for (int bulletCount = 0; bulletCount < _bulletPerEmitter; bulletCount++)
                {
                    BulletBase bullet = (await GameObjectSpawner.Instance.GetAsync(bulletRef.RuntimeKey.ToString(), EPool.Projectile)).GetComponent<BulletBase>();
                    emitter.Shoot(
                        this,
                        weapon,
                        bullet, 
                        trackingTarget==null?null: trackingTarget.GetMidTransform(),
                        bulletVelocity, bulletSize.FloatValue,
                        offset: ((bulletCount - _bulletPerEmitter / 2) * _emitOffset + (_bulletPerEmitter % 2 == 0 ? _emitOffset / 2f : 0)),
                        actorTag ,
                        delayShoot: count * DelayShootEachBullet.FloatValue);
                    onShot?.Invoke();
                    count++;
                }
                if (_fireRate != 0)
                {
                    await UniTask.Delay((int)((1 / _fireRate) * 1000), cancellationToken: cancellation.Token);
                }
            }
            if (_loopSpacing != 0)
                await UniTask.Delay((int)(_loopSpacing * 1000), cancellationToken: cancellation.Token);
        }
    }

   

   
    public static Vector2 RotationToVector(float rotationAngle)
    {
        return new Vector2(Mathf.Cos(rotationAngle * Mathf.Deg2Rad), Mathf.Sin(rotationAngle * Mathf.Deg2Rad));
    }
    public static float AngleBetweenVectors(Vector2 from, Vector2 to)
    {
        return Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
    }

}
[System.Serializable]
public class ProjectileEmitter
{
    public Vector2 position;
    public Vector2 direction;
    public float power = 1;

    public void Shoot(ShootPatternBase shootPattern,WeaponBase weapon,BulletBase bullet,Transform target, float bulletVelocity,float bulletSize,float offset,string tag,float delayShoot)
    {
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (shootPattern.GetStat(EPatternStat.Gravity, 0) != 0)
        {
            rb.gravityScale = shootPattern.GetStat(EPatternStat.Gravity, 0);
        }
        bullet.Velocity = bulletVelocity;
        bullet.Size = bulletSize;
        bullet.SetUp(weapon, position+Vector2.Perpendicular(direction)*offset, direction*power, tag, target: target,delay:delayShoot);
    }
}