using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using Sirenix.OdinInspector;
using Spine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangeShotTask : SkillTask
{
    public bool RunOnStart = false;
    public string animationSkill = "attack/combo_1";
    public string VFX_Name = "";

    [BoxGroup("Range Shot")] public BulletSimpleDamageObject bulletPrefab;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletSize;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletVelocity;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletDmg;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletReflect;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletNumber;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletChaseLevel;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletDelayBtwShot = new ValueConfigSearch("[{0}]BulletDelayBtwShot", "0.2");
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletDelayRelease = new ValueConfigSearch("", "0");


    [BoxGroup("Range Shot")] public bool bulletIsForcusTarget = true;
    [BoxGroup("Range Shot")] public Transform pos;
    [BoxGroup("Range Shot")] public Transform targetTrackingBone;

    [BoxGroup("Range Shot")]
    [Header("Noise Bullet")]
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletAngleZone;
    [BoxGroup("Range Shot")] public ValueConfigSearch bulletAngleOffset;

    public UnityEvent eventShoot;
    private Vector3 originTrackingBone;
    private void Awake()
    {
        if (targetTrackingBone != null)
        {
            originTrackingBone = targetTrackingBone.position;
        }
    }


    public override async UniTask Begin()
    {
        await base.Begin();
        if (GameController.Instance.GetMainActor() == null)
        {
            IsCompleted = true;
            return;
        }
        bulletSize = bulletSize.SetId(Caster.gameObject.name);
        bulletVelocity = bulletVelocity.SetId(Caster.gameObject.name);
        bulletDmg = bulletDmg.SetId(Caster.gameObject.name);
        bulletReflect = bulletReflect.SetId(Caster.gameObject.name);
        bulletNumber = bulletNumber.SetId(Caster.gameObject.name);
        bulletChaseLevel = bulletChaseLevel.SetId(Caster.gameObject.name);
        bulletAngleZone = bulletAngleZone.SetId(Caster.gameObject.name);
        bulletAngleOffset = bulletAngleOffset.SetId(Caster.gameObject.name);

        if (RunOnStart)
        {
            OnAttackTracking(pos);
            return;
        }

        Caster.AnimationHandler.SetAnimation(1, animationSkill, false);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
    }

    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        return base.End();
    }
    public override void OnStop()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        base.OnStop();
    }

    protected virtual void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            IsCompleted = true;
        }
    }

    protected virtual void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                OnAttackTracking(pos);
            }
        }
    }

    protected virtual void OnAttackTracking(Transform pos)
    {
        eventShoot?.Invoke();
        ReleaseBullet(pos);
    }
    protected virtual void SpawnVFXMuzzle(Vector3 pos)
    {
        if (!string.IsNullOrEmpty(VFX_Name))
        {
            GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
            {
                t.GetComponent<Game.Effect.EffectAbstract>().Active(pos);
            });
        }
    }

    protected virtual BulletSimpleDamageObject ReleaseBullet(Transform pos, float angle)
    {
        SpawnVFXMuzzle(pos.position);

        if (bulletAngleZone.FloatValue != 0)
        {
            var offset = 0f;
            var centerAngle = GetAngleToTarget();
            var upAngle = centerAngle + bulletAngleZone.FloatValue;
            var downAngle = centerAngle - bulletAngleZone.FloatValue;
            float angleRdCal = 0;
            for (int j = 0; j < 100; j++)
            {
                angleRdCal = Random.Range(bulletAngleOffset.FloatValue, bulletAngleZone.FloatValue + 10);
                var up = angle + angleRdCal;
                var down = angle - angleRdCal;

                if (up > upAngle)
                {
                    continue;
                }
                if (down < downAngle)
                {
                    continue;
                }

                if (Random.value < 0.5f)
                {
                    offset = angleRdCal;
                }
                else
                {
                    offset = -angleRdCal;
                }
                break;

            }
            angle += offset;
        }

        var bullet = PoolManager.Instance.Spawn(bulletPrefab);
        bullet.transform.position = pos.position;

        if (bulletIsForcusTarget)
        {
            bullet.transform.eulerAngles = Vector3.forward * angle;
        }

        var bullet_size = bulletSize.Vector2Value;
        float bullet_size_Fix = bulletSize.FloatValue;
        if (bullet_size != Vector2.zero)
        {
            bullet_size_Fix = Random.Range(bullet_size.x, bullet_size.y);
        }

        bullet.transform.localScale = Vector3.one * bullet_size_Fix;


        var bullet_velocity = bulletVelocity.Vector2Value;
        float bullet_velocity_Fix = bulletVelocity.FloatValue;
        if (bullet_velocity != Vector2.zero)
        {
            bullet_velocity_Fix = Random.Range(bullet_velocity.x, bullet_velocity.y);
        }

        var statSpeed = new Stat(bullet_velocity_Fix);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        bullet.Movement.Speed = statSpeed;
        bullet.SetCaster(Caster);
        bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.SetId(Caster.gameObject.name).FloatValue);
        bullet.SetMaxHit(bulletReflect.IntValue > 1 ? bulletReflect.IntValue : 1);
        bullet.SetMaxHitToTarget(1);
        bullet.Movement.TrackTarget(bulletChaseLevel.FloatValue, GameController.Instance.GetMainActor().transform);
        DelayRelease(bullet, bulletDelayRelease.FloatValue);


        return bullet;
    }

    protected virtual async void DelayRelease(BulletSimpleDamageObject bullet, float delay)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        bullet.Play();
    }

    public float GetAngleToTarget()
    {
        var target = Caster.FindClosetTarget();
        if (target == null) return 0;
        var angle = GameUtility.GameUtility.GetAngle(pos, target.GetMidTransform());

        return angle;
    }
    protected virtual void ReleaseBullet(Transform pos)
    {
        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            target = GameController.Instance.GetMainActor();
        }
        if (target == null) return;
        var angle = GameUtility.GameUtility.GetAngle(Caster, target);
        Timing.RunCoroutine(_Shot(pos), gameObject);
    }

    protected virtual IEnumerator<float> _Shot(Transform pos)
    {
        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            target = GameController.Instance.GetMainActor();
        }
        if (target != null)
        {
            for (int i = 0; i < (bulletNumber.IntValue == 0 ? 1 : bulletNumber.IntValue); i++)
            {
                var angle = GameUtility.GameUtility.GetAngle(pos, target.GetMidTransform());
                ReleaseBullet(pos, angle);
                yield return Timing.WaitForSeconds(bulletDelayBtwShot.FloatValue);
            }
        }
        if (RunOnStart)
        {
            IsCompleted = true;
        }
    }

    public override void Run()
    {
        base.Run();
        if (targetTrackingBone != null)
        {
            var target = Caster.FindClosetTarget();
            if (target != null)
            {
                targetTrackingBone.position = Caster.FindClosetTarget().GetMidTransform().position;
            }
            else
            {
                targetTrackingBone.position = originTrackingBone;
            }
        }
    }
}