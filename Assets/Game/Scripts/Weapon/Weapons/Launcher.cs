using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu (menuName ="Gun/Launcher")]
public class Launcher : GunBase
{
    public float throwAngle = 20f;


    //private ValueConfigSearch Velocity;

    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        Launcher instance = (Launcher)await base.SetUp(character);
        instance.throwAngle = throwAngle;
        return instance;
    }


   
    Transform triggerPos;
    ITarget target;
    public override bool Trigger(Transform triggerPos, Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        this.triggerPos = triggerPos;
        this.target = trackingTarget;
        this.onEnded = onEnded;
        if (Time.time - lastTrigger >= 1 / GetFireRate())
        {
            this.onEnded = onEnded;
            PlayTriggerSFX();
            TriggerPattern().Forget();

            lastTrigger = Time.time;

            return true;
        }
        return false;
    }

   

    List<UniTask> patternTasks = new List<UniTask>();
    async UniTask TriggerPattern()
    {
        patternTasks.Clear();
        float angleTarget = Vector3.Angle(target.GetMidTransform().position - triggerPos.position, new Vector3(target.GetMidTransform().position.x > triggerPos.position.x ? 1 : -1, 0));
        float finalAngle = target.GetMidTransform().position.y < triggerPos.position.y ? throwAngle : (Mathf.Clamp(angleTarget + 10, throwAngle, 86));

        foreach (var pattern in shootPatternInstances)
        {
            Spread_ShootPattern p = (Spread_ShootPattern)pattern;
            float gravityScale = GetBulletVelocity();
            pattern.AddStat(EPatternStat.Gravity, gravityScale);

            patternTasks.Add(pattern.Trigger(this,
                triggerPos,gravityScale,
                target:null,
                GameUtility.GameUtility.CalcBallisticVelocityVector(triggerPos.position, target.GetMidTransform().position, finalAngle, gravityScale: gravityScale),
                target, bulletRef,
                onShot: () =>
                {
                    base.PlayShotSFX();
                    base.PlayTriggerImpact();
                }));
        }
        await UniTask.WhenAll(patternTasks);

        OnAttackEnded();

    }




}
