using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu (menuName ="Gun/GunData")]
public class GunBase : WeaponBase
{
    public AudioClip[] fireSFXs;
    public AssetReferenceGameObject bulletRef,weaponRef;
    public float soundDelay = 0.2f;
    public float soundVolumn = 0.45f;
    protected CancellationTokenSource cancellation;

    public List<ShootPatternBase> shootPatterns;
    protected List<ShootPatternBase> shootPatternInstances;


    public WeaponBase[] attachedWeapons;

    //player will equip this weapon obj
    public Weapon weaponObj;
    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        GunBase instance = (GunBase)await base.SetUp(character);
        if (weaponRef!=null &&weaponRef.RuntimeKeyIsValid())
        {
            instance.weaponObj = (await Addressables.InstantiateAsync(weaponRef)).GetComponent<Weapon>();
            instance.weaponObj.SetUp(character, this);
        }
        instance.bulletRef = bulletRef;
        instance.baseDamage = baseDamage;
        instance.fireSFXs = fireSFXs;
        instance.soundVolumn = soundVolumn;
        instance.attachedWeapons = new WeaponBase[attachedWeapons.Length];
        for(int i = 0; i < attachedWeapons.Length; i++)
        {
            instance.attachedWeapons[i] = await attachedWeapons[i].SetUp(character);
        }
        instance.cancellation = new CancellationTokenSource();

        instance.shootPatternInstances = new List<ShootPatternBase>();
        foreach (var pattern in shootPatterns)
        {
            instance.shootPatternInstances.Add(await pattern.SetUp(character));
        }
        return instance;
    }

    protected int index = 0;
    protected async UniTask< BulletBase> GetBullet()
    {
        return (await GameObjectSpawner.Instance.GetAsync(bulletRef.RuntimeKey.ToString())).GetComponent<BulletBase>();
    }
    public override Transform GetShootPoint()
    {
        return weaponObj!=null?weaponObj.GetShootPoint():character.WeaponHandler.GetAttackPoint(this);
    }

    protected bool isInitShake = false;
    protected float lastPlayClip = 0;
    protected bool isReloading = false;

   
    public override bool Trigger(Transform triggerPos,Transform target,Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        if (CanTrigger()  )
        {
            this.onEnded = onEnded;
            //
            weaponObj?.Trigger();
            lastTrigger = Time.time;

            TriggerPattern(triggerPos, target,facing,trackingTarget);

            return true;
        }
        return false;
    }
    List<UniTask> patternTasks = new List<UniTask>();
    protected async UniTask TriggerPattern(Transform triggerPos, Transform target,Vector2 facing, ITarget trackingTarget)
    {
        patternTasks.Clear();
        int index = 0;
        float effectTime = 0;
        foreach (var pattern in shootPatternInstances)
        {
            patternTasks.Add(pattern.Trigger(this, triggerPos,GetBulletVelocity(), target, facing, trackingTarget, bulletRef, 
                onShot: () =>
                {
                    if (Time.time - effectTime > 0.2f)
                    {
                        PlayTriggerSFX();
                        PlayTriggerImpact();
                        effectTime = Time.time;
                    }
                }
            ));
            index++;
        }
        await UniTask.WhenAll(patternTasks);

        OnAttackEnded();
    }

    //
    public override void Destroy()
    {
        base.Destroy();
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }
        for(int i = 0; i < shootPatternInstances.Count; i++)
        {
            shootPatternInstances[i].Destroy();
            Destroy(shootPatternInstances[i]);
        }
        shootPatternInstances.Clear();
        if(weaponObj!=null)
        Addressables.ReleaseInstance(weaponObj.gameObject);
        Destroy(this);
    }

    public override void Release()
    {
        isInitShake = false;
    }

    public override void PlayTriggerSFX()
    {
        if (fireSFXs != null && fireSFXs.Length > 0)
        {
            character.SoundHandler.PlayOneShot(fireSFXs[Random.Range(0, fireSFXs.Length)], soundVolumn);
            lastPlayClip = Time.time;
        }
    }
    public override void PlayTriggerImpact()
    {
        weaponObj?.PlayTriggerImpact(knockBackTime,knockBack);
    }


    public async UniTask AddShootPattern(ShootPatternBase pattern)
    {
        shootPatternInstances.Add(await pattern.SetUp(character));
    }

    public List<ShootPatternBase> GetShootPattern()
    {
        return shootPatternInstances;
    }
}