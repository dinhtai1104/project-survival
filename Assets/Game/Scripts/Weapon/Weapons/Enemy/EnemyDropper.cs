using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "EnemyWeapon/EnemyDropper")]
public class EnemyDropper : WeaponBase
{
    public AudioClip[] fireSFXs;
    public AudioClip fireStartSFX;

    public AssetReferenceGameObject [] bulletRefs;
    protected CancellationTokenSource cancellation;

    public List<ShootPatternBase> shootPatterns; 
    private List<ShootPatternBase> shootPatternInstances;

    public AssetReference muzzleEffectRef;
    public AssetReference preTriggerEffectRef;

    public bool eventTrigger = true;

    //private ValueConfigSearch Velocity;

    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        EnemyDropper instance = (EnemyDropper)await base.SetUp(character);
        instance.baseDamage = baseDamage;
        instance.fireSFXs = fireSFXs;
        instance.fireStartSFX = fireStartSFX;
        instance.bulletRefs = bulletRefs;
        instance.eventTrigger = eventTrigger;
        instance.muzzleEffectRef = muzzleEffectRef;
        instance.preTriggerEffectRef = preTriggerEffectRef;
        instance.cancellation = new CancellationTokenSource();
        instance.shootPatternInstances = new List<ShootPatternBase>();

        foreach (var pattern in shootPatterns)
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

        character.SoundHandler.PlayOneShot(fireStartSFX, 0.8f);
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
    ITarget trackingTarget;
    Transform target;
    Vector3 facing;
    public override bool Trigger(Transform triggerPos, Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        this.facing = facing;
        this.triggerPos = triggerPos;
        this.trackingTarget = trackingTarget;
        this.target = target;
        this.onEnded = onEnded;
        if (CanTrigger())
        {
            lastTrigger = Time.time;
            
            this.onEnded = onEnded;


            if (preTriggerEffectRef != null && !string.IsNullOrEmpty(preTriggerEffectRef.RuntimeKey.ToString()))
            {
                Game.Pool.GameObjectSpawner.Instance.Get(preTriggerEffectRef.RuntimeKey.ToString(), obj =>
                {
                    obj.GetComponent<Game.Effect.EffectAbstract>().Active(triggerPos.position).SetParent(triggerPos);
                });
            }

            Logger.Log("TRIGGER " + triggerPos.gameObject.name);
            if (eventTrigger)
            {
                character.AnimationHandler.onTriggerEvent += OnEvent;

            }
            else
            {
                PlayTriggerSFX();
                TriggerPattern();
            }

            return true;
        }
        return false;
    }
   
    void OnEvent(Spine.Event e)
    {
        character.AnimationHandler.onTriggerEvent -= OnEvent;
        if (e.Data.Name.Equals("attack_tracking"))
        {
            PlayTriggerSFX();
            TriggerPattern();
        }
    }

    List<UniTask> patternTasks = new List<UniTask>();
    async UniTask TriggerPattern()
    {
        patternTasks.Clear();
        int index = 0;

        foreach (var pattern in shootPatternInstances)
        {
            Spread_ShootPattern p = (Spread_ShootPattern)pattern;

            patternTasks.Add(pattern.Trigger(this,
                triggerPos,
                bulletVelocity:1,
                target: target,
                facing:this.facing,
                trackingTarget:trackingTarget, 
                bulletRefs[index],
                onShot: () =>
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
            })) ;
            index++;
        }
        await UniTask.WhenAll(patternTasks);

        OnAttackEnded();

    }


  

}
