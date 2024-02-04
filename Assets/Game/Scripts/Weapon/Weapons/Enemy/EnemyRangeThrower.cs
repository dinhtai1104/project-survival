using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "EnemyWeapon/EnemyRangeThrower")]
public class EnemyRangeThrower : WeaponBase
{
    public float throwAngle=45f;
    public AudioClip[] fireSFXs;
    public AudioClip fireStartSFX;

    public AssetReferenceGameObject [] bulletRefs;
    protected CancellationTokenSource cancellation;

    public List<ShootPatternBase> shootPatterns; 
    private List<ShootPatternBase> shootPatternInstances;
    public bool eventTrigger = true;

    //private ValueConfigSearch Velocity;

    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        EnemyRangeThrower instance = (EnemyRangeThrower)await base.SetUp(character);
        instance.baseDamage = baseDamage;
        instance.fireSFXs = fireSFXs;
        instance.fireStartSFX = fireStartSFX;
        instance.bulletRefs = bulletRefs;
        instance.eventTrigger = eventTrigger;
        instance.cancellation = new CancellationTokenSource();
        instance.shootPatternInstances = new List<ShootPatternBase>();

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
        if (CanTrigger())
        {
            this.onEnded = onEnded;
            lastTrigger = 999999;
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
        float angleTarget = Vector3.Angle(target.position - triggerPos.position, new Vector3(target.position.x > triggerPos.position.x ? 1 : -1, 0)); 
        float finalAngle = target.position.y<triggerPos.position.y?throwAngle:(Mathf.Clamp(angleTarget+10, throwAngle,86));

        foreach (var pattern in shootPatternInstances)
        {
            Spread_ShootPattern p = (Spread_ShootPattern)pattern;
            float gravityScale = GetBulletVelocity()*GameTime.Controller.TIME_SCALE;
            pattern.AddStat(EPatternStat.Gravity, gravityScale);

            patternTasks.Add(pattern.Trigger(this,
                triggerPos,
                bulletVelocity:gravityScale,
                target: target,
                facing:GameUtility.GameUtility.CalcBallisticVelocityVector(triggerPos.position, target.position, finalAngle, gravityScale: gravityScale),
                trackingTarget:trackingTarget, 
                bulletRefs[index],
                onShot: () =>
            {
                PlayShotSFX();
                PlayTriggerImpact();
            })) ;
            index++;
        }
        await UniTask.WhenAll(patternTasks);

        lastTrigger = Time.time;
        OnAttackEnded();

    }


  

}
