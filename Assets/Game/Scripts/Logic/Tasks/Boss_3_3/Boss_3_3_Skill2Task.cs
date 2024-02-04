using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GameActor;
using Game.Pool;
using Spine;
using System.Security.Cryptography;
using UnityEngine;

public class Boss_3_3_Skill2Task : SkillTask
{
    public string animationSkill;
    public string VFX_Name;
    public BulletSimpleDamageObject bulletPrefab;
    public Transform shotPos;

    public ValueConfigSearch bullet_Size;
    public ValueConfigSearch bullet_Dmg;
    public ValueConfigSearch bullet_Velocity;

    public LayerMask groundMask;
    public GameObject predictPrefab;
    private Vector3[] _pts;
    public override async UniTask Begin()
    {
        await base.Begin();
        _pts = new Vector3[5];
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }
        Caster.AnimationHandler.SetAnimation(0,animationSkill, false);
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

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
        }
    }


    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                if (!string.IsNullOrEmpty(VFX_Name))
                {
                    GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
                    {
                        t.GetComponent<Game.Effect.EffectAbstract>().Active(Caster.WeaponHandler.DefaultAttackPoint.position);
                    });
                }
                ReleaseBullet();
                IsCompleted = true;

            }
        }
    }

    private void ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        if (target != null)
        {
            var bullet = PoolManager.Instance.Spawn(bulletPrefab);
            bullet.transform.position = shotPos.position;
            bullet.SetCaster(Caster);
            bullet.SetMaxHit(1);
            bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bullet_Dmg.SetId(Caster.gameObject.name).FloatValue);
            bullet.transform.localScale = Vector3.one * bullet_Size.SetId(Caster.gameObject.name).FloatValue;

            var dir = GameUtility.GameUtility.CalcBallisticVelocityVector(shotPos.position, target.GetPosition(), 45, bullet_Velocity.SetId(Caster.gameObject.name).FloatValue);
            var end = FindTargetPos(dir, shotPos.position, out var lastDir, out var curPos);
            if (end != null)
            {
                var predict = PoolManager.Instance.Spawn(predictPrefab);
                var pos = curPos - lastDir;
                Vector3 rotation = Vector3.zero;
                // Wall check
                if (Physics2D.Raycast(pos, Vector3.down, 1))
                {
                    rotation = Vector3.zero;
                }
                else if (Physics2D.Raycast(pos, Vector3.left, 1))
                {
                    rotation = new Vector3(0, 0, -90);
                }
                else
                {
                    rotation = new Vector3(0, 0, 90);
                }
                predict.transform.eulerAngles = rotation;

                predict.transform.position = curPos;
            }
            bullet.Movement.Move(new Stat(0), dir);
            bullet.Play();
        }
    }


    public Collider2D FindTargetPos(Vector2 velocity, Vector2 startPos, out Vector2 lastDir, out Vector2 curPos)
    {
        var pos = startPos;
        var vel = velocity;
        var gravity = Physics2D.gravity;
        int max = 10000;
        while (max-- > 0)
        {
            pos += vel * Time.deltaTime;
            vel += gravity * Time.deltaTime;

            var hit = Physics2D.OverlapCircle(pos, 1f, groundMask);
            if (hit != null)
            {
                curPos = pos; 
                lastDir = vel;
                return hit;
            }
        }
        curPos = pos;
        lastDir = vel;
        return null;
    }
}