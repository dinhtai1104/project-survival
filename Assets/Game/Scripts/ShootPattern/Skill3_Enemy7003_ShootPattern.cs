using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "ShootPattern/7003Skill3_ShootPattern")]
public class Skill3_Enemy7003_ShootPattern : ShootPatternBase
{
    [SerializeField]
    private Vector2 [] offsets,directions;

    public ValueConfigSearch BulletSize;
    private string actorTag;
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
        Skill3_Enemy7003_ShootPattern instance = (Skill3_Enemy7003_ShootPattern)await base.SetUp(actor);
        instance.offsets = offsets;
        instance.directions = directions;
        instance.BulletSize = BulletSize.Clone().SetId(actor.gameObject.name);
        instance.actorTag = actor.gameObject.tag;
        for (int i = 0; i < instance.offsets.Length; i++)
        {
            ProjectileEmitter emitter = new ProjectileEmitter();
            instance.emitters.Add(emitter);
        }


        return instance;

    }
    public override void Init(ActorBase actor)
    {
        cancellation = new CancellationTokenSource();

    }

    public override async UniTask Release()
    {

    }

    public override async UniTask Trigger(WeaponBase weapon, Transform triggerPos, float bulletVelocity, Transform target, Vector2 facing, ITarget trackingTarget, AssetReferenceGameObject bulletRef, System.Action onShot)
    {
        if (weapon.character.IsDead()) return;
      
        Vector2 triggerPosition = (Vector2)triggerPos.position;
        for (int i = 0; i < emitters.Count; i++)
        {
            ProjectileEmitter emitter = emitters[i];

            emitter.direction = directions[i] ;
            emitter.position = triggerPosition + offsets[i];
            emitter.power = facing.magnitude == 0 ? 1 : facing.magnitude;


            BulletBase bullet = (await GameObjectSpawner.Instance.GetAsync(bulletRef.RuntimeKey.ToString(), EPool.Projectile)).GetComponent<BulletBase>();
            emitter.Shoot(
                this,
                weapon,
                bullet,
                trackingTarget == null ? null : trackingTarget.GetMidTransform(),
                bulletVelocity, BulletSize.FloatValue,
                offset: 0,
                actorTag,
                delayShoot: 0);
            onShot?.Invoke();
           
            
        }
    }





}
