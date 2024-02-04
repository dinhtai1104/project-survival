using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "ShootPattern/PlayerBasedDirection_Spread_ShootPattern")]
public class PlayerBasedDirection_Spread_ShootPattern : Spread_ShootPattern
{
   

    CancellationTokenSource cancellation;

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
        PlayerBasedDirection_Spread_ShootPattern instance = (PlayerBasedDirection_Spread_ShootPattern)await base.SetUp(actor);
        

        return instance;

    }
    public override void Init(ActorBase actor)
    {
        cancellation = new CancellationTokenSource();
       
    }

    public override async UniTask Release()
    {
    }

    //public override async UniTask Trigger(WeaponBase weapon, Transform triggerPos, float bulletVelocity, Vector2 facing, ITarget trackingTarget, AssetReferenceGameObject bulletRef, System.Action onShot)
    //{
    //    await Trigger(weapon, triggerPos, bulletVelocity,target: null, facing, bulletRef, onShot);
    //}
    Vector2 left = Vector2.left;
    Vector2 right = Vector2.right;
    public override async UniTask Trigger(WeaponBase weapon, Transform triggerPos, float bulletVelocity, Transform target,Vector2 facing, ITarget trackingTarget, AssetReferenceGameObject bulletRef, Action onShot)
    {
        int count = 0;
        for (int l = 0; l < _loop;l++) 
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                ProjectileEmitter emitter = emitters[i];
                emitter.direction = weapon.character.GetLookDirection().x > 0 ? left : right;
                emitter.position = (Vector2)weapon.character.GetMidTransform().position ;
                emitter.power = facing.magnitude;


                for (int bulletCount = 0; bulletCount < _bulletPerEmitter; bulletCount++)
                {
                    BulletBase bullet = (await GameObjectSpawner.Instance.GetAsync(bulletRef.RuntimeKey.ToString(), EPool.Projectile)).GetComponent<BulletBase>();
                    emitter.Shoot(this,weapon, bullet, target, bulletVelocity, bulletSize.FloatValue,offset: ((bulletCount-_bulletPerEmitter/2) * _emitOffset+(_bulletPerEmitter % 2==0?_emitOffset/2f:0)),actorTag,
                        delayShoot: count * DelayShootEachBullet.FloatValue);
                    onShot?.Invoke();
                    count++;
                }
                if (_fireRate != 0)
                {
                    await UniTask.Delay((int)((1 / _fireRate) * 1000), cancellationToken: cancellation.Token);
                }
            }
            if(_loopSpacing!=0)
                await UniTask.Delay((int)(_loopSpacing * 1000), cancellationToken: cancellation.Token);
        }
    }

   

}

