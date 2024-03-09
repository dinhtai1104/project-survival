using System.Collections;
using System.Collections.Generic;
using Engine;
using ExtensionKit;
using Pool;
using Spine;
using UnityEngine;
using UnityEngine.Events;

public class RangeTask : SkillTask
{
    [SerializeField] private string m_Animation;
    [SerializeField] protected Transform m_FirePoint;
    [SerializeField] private string m_EventName;
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private DamageDealer m_DamageDealer;
    [SerializeField] private Vector2 m_NoiseAngle;
    [SerializeField] private bool m_SyncTimeScale;
    [SerializeField] private bool m_LootAtTarget = true;
    [SerializeField] private UnityEvent m_OnSpawnBullet;
    [SerializeField] private UnityEvent<Bullet2D> m_OnStartBullet;

    private Spine.EventData m_EventData;

    [SerializeField] protected Stat DefaultSpeed = new Stat(10);

    public GameObject BulletPrefab
    {
        get { return m_BulletPrefab; }
    }

    public override void Begin()
    {
        DefaultSpeed = ConstantValue.DefaultSpeed;
        if (m_EventName.IsNotNullAndEmpty())
        {
            m_EventData = Caster.Animation.FindEvent(m_EventName);

            if (m_EventData != null) Caster.Animation.SubscribeEvent(OnAttack);
        }

        m_DamageDealer.Init(Caster.Stats);
        m_DamageDealer.DamageSource.Value = Caster.Stats.GetValue(StatKey.Damage);
        base.Begin();
        if (!m_Animation.IsNotNullAndEmpty() || !m_EventName.IsNotNullAndEmpty())
        {
            Attack();
            IsCompleted = true;
        }

        if (m_LootAtTarget && Caster.AI && Caster.TargetFinder.CurrentTarget != null)
        {
            Caster.Movement.LookAt(Caster.TargetFinder.CurrentTarget.Trans.position);
        }
    }

    public override void Run()
    {
        base.Run();
        var anim = Caster.Animation;
        if (m_Animation.IsNotNullAndEmpty() && anim.EnsurePlay(0, m_Animation, false))
        {
            if (anim.IsCurrentAnimationComplete)
            {
                IsCompleted = true;
            }
        }
    }

    public override void End()
    {
        base.End();
        if (m_SyncTimeScale)
        {
            Caster.Animation.TimeScale = 1f;
        }
        if (m_EventData != null) Caster.Animation.UnsubcribeEvent(OnAttack);
    }
    public override void Interrupt()
    {
        base.Interrupt();
        if (m_SyncTimeScale)
        {
            Caster.Animation.TimeScale = 1f;
        }
        if (m_EventData != null) Caster.Animation.UnsubcribeEvent(OnAttack);
    }
    protected virtual void Attack()
    {
        var target = Caster.TargetFinder.CurrentTarget;
        if (target == null) return;
        if (m_BulletPrefab == null) return;
        var dir = target.CenterPosition - m_FirePoint.position;

        var quaternionTarget = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        CreateBullet(quaternionTarget);
    }

    protected virtual Bullet2D CreateBullet(Quaternion angle)
    {
        var firePosition = m_FirePoint.position;
        var fireRotation = angle;

        GameObject gameObject = PoolManager.Instance.Spawn(m_BulletPrefab, firePosition, fireRotation);

        Bullet2D bullet = gameObject.GetComponent<Bullet2D>();
        bullet.Owner = Caster;
        bullet.TargetLayer = Caster.EnemyLayerMask;


        // Add rotation noise
        Vector3 eulerAngles = bullet.Trans.eulerAngles;
        eulerAngles.z += Random.Range(m_NoiseAngle.x, m_NoiseAngle.y);
        bullet.Trans.rotation = Quaternion.Euler(eulerAngles);
        var target = Caster.TargetFinder.CurrentTarget;
        if (target != null)
        {
            bullet.Target = target.Trans;
            bullet.TargetPosition = target.CenterPosition;
        }

        if (m_DamageDealer != null)
        {
            bullet.DamageDealer?.CopyData(m_DamageDealer);
        }
        m_OnStartBullet.Invoke(bullet);
        bullet.SetSpeed(DefaultSpeed);
        SetupBullet(bullet);
        bullet.StartBullet();

        return bullet;
    }

    protected virtual void SetupBullet(Bullet2D bullet) { }

    private void OnAttack(TrackEntry trackEntry, Spine.Event e)
    {
        if (!IsRunning || m_EventData != e.Data || !Caster.Animation.IsPlaying(m_Animation)) return;

        m_OnSpawnBullet.Invoke();
        Attack();
    }
}