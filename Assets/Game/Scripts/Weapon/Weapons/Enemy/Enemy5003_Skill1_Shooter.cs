using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "EnemyWeapon/Enemy5003_Skill1_Shooter")]
public class Enemy5003_Skill1_Shooter : WeaponBase
{


    public AudioClip fireStartSFX;
    public AudioClip[] fireSFXs;

    public AssetReferenceGameObject[] bulletRefs;
    protected CancellationTokenSource cancellation;

    public List<ShootPatternBase> shootPatterns;
    private List<ShootPatternBase> shootPatternInstances;

    public bool eventTriggerShot = true;


    public ValueConfigSearch MinRotation, MaxRotation, RotateSpeed;
    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        Enemy5003_Skill1_Shooter instance = (Enemy5003_Skill1_Shooter)await base.SetUp(character);

        instance.baseDamage = baseDamage;
        instance.fireSFXs = fireSFXs;
        instance.fireStartSFX = fireStartSFX;
        instance.bulletRefs = bulletRefs;
        instance.eventTriggerShot = eventTriggerShot;
        instance.MinRotation = MinRotation;
        instance.MaxRotation = MaxRotation;
        instance.RotateSpeed = RotateSpeed;
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
    Vector2 facing;
    Transform target;
    ITarget trackingTarget;
    public override bool Trigger(Transform triggerPos, Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        this.triggerPos = triggerPos;
        this.facing = facing;
        this.onEnded = onEnded;
        this.target = target;
        this.trackingTarget = trackingTarget;

        if (Time.time - lastTrigger >= 1 / GetFireRate())
        {
            Debug.DrawLine(triggerPos.position, triggerPos.position + (Vector3)facing * 10, Color.red, 2);
            this.onEnded = onEnded;
            lastTrigger = 999999;
            if (eventTriggerShot)
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
        if (e.Data.Name.Equals("attack_tracking"))
        {
            PlayTriggerSFX();
            TriggerPattern();
            character.AnimationHandler.onTriggerEvent -= OnEvent;
        }
    }
    List<UniTask> patternTasks = new List<UniTask>();
    private float targetRotation,targetRotateSpeed;
    async UniTask TriggerPattern()
    {

        targetRotation = UnityEngine.Random.Range(MinRotation.IntValue, MaxRotation.IntValue);
        targetRotateSpeed = RotateSpeed.FloatValue;

        Messenger.AddListener<BulletMoveSpiralHandler>(EventKey.OnBulletMove, OnBulletMove);

        patternTasks.Clear();
        int index = 0;
        foreach (var pattern in shootPatternInstances)
        {
            patternTasks.Add(pattern.Trigger(this, triggerPos, GetBulletVelocity(), target, facing, trackingTarget, bulletRefs[index], onShot: () =>
            {
                PlayShotSFX();
                PlayTriggerImpact();
            }));
            index++;
        }
        await UniTask.WhenAll(patternTasks);
        lastTrigger = Time.time;
        OnAttackEnded();

        await UniTask.Delay(1000,cancellationToken:cancellation.Token);
        Messenger.RemoveListener<BulletMoveSpiralHandler>(EventKey.OnBulletMove, OnBulletMove);

    }

    private void OnBulletMove(BulletMoveSpiralHandler moveHandler)
    {
        moveHandler.targetRotation = (int)targetRotation;
        moveHandler.rotateSpeed = targetRotateSpeed;
    }

   


}