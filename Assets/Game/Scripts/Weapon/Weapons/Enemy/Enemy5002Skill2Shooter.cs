using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "EnemyWeapon/Enemy5002Skill2Shooter")]
public class Enemy5002Skill2Shooter : WeaponBase
{
    public float throwAngle = 45f;
    public AudioClip[] fireSFXs;
    public AudioClip fireStartSFX;

    public AssetReferenceGameObject[] bulletRefs;
    protected CancellationTokenSource cancellation;

    public List<ShootPatternBase> shootPatterns;
    private List<ShootPatternBase> shootPatternInstances;
    public AssetReference muzzleEffectRef;

    public ValueConfigSearch Gravity;

    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        Enemy5002Skill2Shooter instance = (Enemy5002Skill2Shooter)await base.SetUp(character);
        instance.baseDamage = baseDamage;
        instance.fireSFXs = fireSFXs;
        instance.fireStartSFX = fireStartSFX;
        instance.bulletRefs = bulletRefs;
        instance.cancellation = new CancellationTokenSource();
        instance.shootPatternInstances = new List<ShootPatternBase>();
        instance.muzzleEffectRef = muzzleEffectRef;
        instance.Gravity = Gravity;

        foreach (var pattern in shootPatterns)
        {
            instance.shootPatternInstances.Add(await pattern.SetUp(character));
        }
        instance.throwAngle = throwAngle;
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
    public override bool Trigger(Transform triggerPos, Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        this.triggerPos = triggerPos;
        this.trackingTarget = trackingTarget;
        this.target = target;
        this.onEnded = onEnded;
        if (Time.time - lastTrigger >= 1 / GetFireRate())
        {
            this.onEnded = onEnded;
            //character.AnimationHandler.onTriggerEvent += OnEvent;
            //lastTrigger = 999999;
            PlayTriggerSFX();
            TriggerPattern();



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
        float finalAngle = throwAngle;

        foreach (var pattern in shootPatternInstances)
        {
            Spread_ShootPattern p = (Spread_ShootPattern)pattern;

            p.AddStat(EPatternStat.Gravity, Gravity.FloatValue);
            patternTasks.Add(pattern.Trigger(this,
                triggerPos,
                bulletVelocity: Gravity.FloatValue,
                target: target,
                facing: Vector2.up*BulletVelocityConfig.FloatValue,
                trackingTarget: trackingTarget,
                bulletRefs[index],
                onShot: () =>
                {
                    PlayShotSFX();
                    PlayTriggerImpact();
                }));
            index++;

            if (muzzleEffectRef != null && !string.IsNullOrEmpty(muzzleEffectRef.RuntimeKey.ToString()))
            {
                Game.Pool.GameObjectSpawner.Instance.Get(muzzleEffectRef.RuntimeKey.ToString(), obj =>
                {
                    obj.GetComponent<Game.Effect.EffectAbstract>().Active(triggerPos.position).SetParent(triggerPos);
                });
            }
        }
        await UniTask.WhenAll(patternTasks);

        lastTrigger = Time.time;
        OnAttackEnded();

    }




}