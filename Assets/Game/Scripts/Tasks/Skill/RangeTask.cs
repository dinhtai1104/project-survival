using System.Collections;
using System.Collections.Generic;
using Engine;
using ExtensionKit;
using Spine;
using UnityEngine;
using UnityEngine.Events;

public class RangeTask : SkillTask
{
    [SerializeField] private string m_Animation;
    [SerializeField] private Transform m_FirePoint;
    [SerializeField] private string m_EventName;
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private DamageDealer m_DamageDealer;
    [SerializeField] private Vector2 m_NoiseAngle;
    [SerializeField] private bool m_SyncTimeScale;
    [SerializeField] private bool m_LootAtTarget = true;
    [SerializeField] private UnityEvent m_OnSpawnBullet;
    [SerializeField] private UnityEvent<Bullet2D> m_OnStartBullet;

    private Spine.EventData m_EventData;

    public GameObject BulletPrefab
    {
        get { return m_BulletPrefab; }
    }

    public override void Begin()
    {
        if (m_EventName.IsNotNullAndEmpty())
        {
            m_EventData = Caster.Animation.FindEvent(m_EventName);

            if (m_EventData != null) Caster.Animation.SubscribeEvent(OnAttack);
        }

        m_DamageDealer.Init(Caster.Stat);
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
    private void Attack()
    {
        if (m_BulletPrefab == null) return;

        var firePosition = m_FirePoint.position;
        var fireRotation = m_FirePoint.rotation;

        GameObject gameObject = PoolManager.Instance.Spawn(m_BulletPrefab, firePosition, fireRotation);
        
        Bullet2D bullet = gameObject.GetComponent<Bullet2D>();
        bullet.Owner = Caster;
        bullet.TargetLayer = Caster.EnemyLayerMask;

        // Add rotation noise
        Vector3 eulerAngles = bullet.Trans.rotation.eulerAngles;
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
        bullet.StartBullet();
    }

    private void OnAttack(TrackEntry trackEntry, Spine.Event e)
    {
        if (!IsRunning || m_EventData != e.Data || !Caster.Animation.IsPlaying(m_Animation)) return;

        m_OnSpawnBullet.Invoke();
        Attack();
    }
}