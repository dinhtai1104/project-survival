using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "EnemyWeapon/EnemyProjectileSummoner")]
public class EnemyProjectileSummoner : WeaponBase
{
    public AudioClip fireStartSFX;
    public AudioClip[] fireSFXs;

    public AssetReferenceGameObject [] bulletRefs;
    protected CancellationTokenSource cancellation;

    public bool eventTriggerShot = true;

    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        EnemyProjectileSummoner instance = (EnemyProjectileSummoner)await base.SetUp(character);
    
        instance.baseDamage = baseDamage;
        instance.fireSFXs = fireSFXs;
        instance.fireStartSFX = fireStartSFX;
        instance.bulletRefs = bulletRefs;
        instance.eventTriggerShot = eventTriggerShot;
        instance.cancellation = new CancellationTokenSource();
    
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
    Vector2 facing;
    ITarget trackingTarget;
    Transform target;
    public override bool Trigger(Transform triggerPos, Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        this.triggerPos = triggerPos;
        this.facing = facing;
        this.onEnded = onEnded;
        this.target = target;
        this.trackingTarget = trackingTarget;
        if (Time.time - lastTrigger >= 1 / GetFireRate())
        {
            this.onEnded = onEnded;
            lastTrigger = 999999;
            if (eventTriggerShot)
            {
                character.AnimationHandler.onTriggerEvent += OnEvent;
            }
            else
            {
                PlayTriggerSFX();
                TriggerShot();
            }

            return true;
        }
        return false;
    }
    //public override bool Trigger(Transform triggerPos, ITarget target, System.Action onEnded)
    //{
    //    this.triggerPos = triggerPos;
    //    this.facing = (target.GetMidTransform().position-triggerPos.position).normalized;
    //    this.onEnded = onEnded;
    //    this.target = target;
    //    if (Time.time - lastTrigger >= 1 / GetFireRate())
    //    {
    //        this.onEnded = onEnded;
    //        lastTrigger = 999999;
    //        if (eventTriggerShot)
    //        {
    //            character.AnimationHandler.onTriggerEvent += OnEvent;
    //        }
    //        else
    //        {
    //            PlayTriggerSFX();
    //            TriggerShot();
    //        }

    //        return true;
    //    }
    //    return false;
    //}
    //public override bool Trigger(Transform triggerPos, System.Action onEnded)
    //{
    //    this.triggerPos = triggerPos;
    //    this.facing = character.GetLookDirection();
    //    this.onEnded = onEnded;
    //    if (Time.time - lastTrigger >= 1 / GetFireRate())
    //    {
    //        this.onEnded = onEnded;
    //        lastTrigger = 999999;
    //        if (eventTriggerShot)
    //        {
    //            character.AnimationHandler.onTriggerEvent += OnEvent;
    //        }
    //        else
    //        {
    //            PlayTriggerSFX();
    //            TriggerShot();
    //        }

    //        return true;
    //    }
    //    return false;
    //}
    void OnEvent(Spine.Event e)
    {
        if (e.Data.Name.Equals("attack_tracking"))
        {
            PlayTriggerSFX();
            TriggerShot();
            character.AnimationHandler.onTriggerEvent -= OnEvent;
        }
    }
  

    void TriggerShot()
    {

    }
  

}