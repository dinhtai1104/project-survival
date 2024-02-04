using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "EnemyWeapon/EnemyRangeShooter")]
public class EnemyRangeShooter : WeaponBase
{
    private const string ATTACK_TRACKING = "attack_tracking";
    public AudioClip fireStartSFX;
    public AudioClip[] fireSFXs;

    public AssetReferenceGameObject [] bulletRefs;
    protected CancellationTokenSource cancellation;

    public List<ShootPatternBase> shootPatterns; 
    private List<ShootPatternBase> shootPatternInstances;

    public bool eventTriggerShot = true,followTarget=true;

    public AssetReference muzzleEffectRef;
    public AssetReference preTriggerEffectRef;

    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        EnemyRangeShooter instance = (EnemyRangeShooter)await base.SetUp(character);
    
        instance.baseDamage = baseDamage;
        instance.fireSFXs = fireSFXs;
        instance.fireStartSFX = fireStartSFX;
        instance.bulletRefs = bulletRefs;
        instance.eventTriggerShot = eventTriggerShot;
        instance.followTarget = followTarget;
        instance.muzzleEffectRef = muzzleEffectRef;
        instance.preTriggerEffectRef = preTriggerEffectRef;
        instance.cancellation = new CancellationTokenSource();
        instance.shootPatternInstances = new List<ShootPatternBase>();
        foreach(var pattern in shootPatterns)
        {
            instance.shootPatternInstances.Add(await pattern.SetUp(character));
        }
        return instance;
    }


   
    public override void Destroy()
    {
        base.Destroy();
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }
        for (int i = 0; i < shootPatternInstances.Count; i++)
        {
            shootPatternInstances[i].Destroy();
            Destroy(shootPatternInstances[i]);
        }
        shootPatternInstances.Clear();

        Destroy(this);
    }

    public override void Release()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }
        cancellation = new CancellationTokenSource();
    }
    float lastPlayClip = 0;
    public override void PlayTriggerSFX()
    {
       
        character.SoundHandler.PlayOneShot(fireStartSFX, 1);
    }
    public override void PlayShotSFX()
    {
        if (fireSFXs != null && fireSFXs.Length > 0)
        {
            character.SoundHandler.PlayOneShot(fireSFXs[Random.Range(0, fireSFXs.Length)], 0.8f);
            lastPlayClip = Time.time;
        }
    }
    public override void PlayTriggerImpact()
    {
    }

    Transform triggerPos;
    Vector2 facing;
    Transform target;
    ITarget trackingTarget;
    public override bool Trigger(Transform triggerPos, Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
       
        if (CanTrigger())
        {
            this.triggerPos = triggerPos;
            this.facing = facing;
            this.onEnded = onEnded;
            this.target = target;
            this.trackingTarget = trackingTarget;
            if (preTriggerEffectRef != null && !string.IsNullOrEmpty(preTriggerEffectRef.RuntimeKey.ToString()))
            {
                Game.Pool.GameObjectSpawner.Instance.Get(preTriggerEffectRef.RuntimeKey.ToString(), obj =>
                {
                    obj.GetComponent<Game.Effect.EffectAbstract>().Active(triggerPos.position).SetParent(triggerPos);
                });
            }
         

            if (eventTriggerShot)
            {
                lastTrigger = 999999;
                character.AnimationHandler.onTriggerEvent += OnEvent;
            }
            else
            {
                lastTrigger = Time.time;
                PlayTriggerSFX();
                TriggerPattern();
            }

            return true;
        }
        return false;
    }
   
    void OnEvent(Spine.Event e)
    {
        if (e.Data.Name.Equals(ATTACK_TRACKING))
        {
            PlayTriggerSFX();
            TriggerPattern();
            character.AnimationHandler.onTriggerEvent -= OnEvent;
        }
    }
    List<UniTask> patternTasks = new List<UniTask>();
    async UniTask TriggerPattern()
    {
        facing = (target==null ||!followTarget)?facing:(Vector2)(target.position - triggerPos.position).normalized;
        patternTasks.Clear();
        int index = 0;
        foreach (var pattern in shootPatternInstances)
        {
            patternTasks.Add(pattern.Trigger(this, triggerPos,GetBulletVelocity(), target, facing,trackingTarget, bulletRefs[index], onShot: () =>
            {
                PlayShotSFX();
                PlayTriggerImpact();
                if (muzzleEffectRef != null && !string.IsNullOrEmpty(muzzleEffectRef.RuntimeKey.ToString()))
                {
                    Game.Pool.GameObjectSpawner.Instance.Get(muzzleEffectRef.RuntimeKey.ToString(), obj =>
                    {
                        obj.GetComponent<Game.Effect.EffectAbstract>().Active(triggerPos.position, (Vector3)facing).SetParent(triggerPos);
                    });
                }
            }));
            index++;
        }
        await UniTask.WhenAll(patternTasks);

        if (eventTriggerShot)
        {
            lastTrigger = Time.time;
        }
        await UniTask.Yield();
        OnAttackEnded();

    }


}
