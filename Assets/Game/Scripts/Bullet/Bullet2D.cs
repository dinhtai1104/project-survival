using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Bullet2D : MonoBehaviour
{
    [SerializeField] private Collider2D m_Collider;
    [SerializeField] private GameObject m_AppearEffect;
    [SerializeField] private GameObject m_ImpactEffect;
    [SerializeField] private GameObject m_CritHitEffect;
    [SerializeField] private DamageDealer m_DamageDealer;
    [SerializeField] private LayerMask m_TargetLayer;
    [SerializeField] private float m_MaxDistance;
    [SerializeField] private float m_MaxDuration;
    [SerializeField] private bool m_DealDamageOnContact = true;
    [SerializeField] private bool m_DestroyOnImpact = true;
    [SerializeField] private bool m_DestroyOnMaximumDistance = true;
    [SerializeField] private bool m_DestroyOnMaximumDuration = false;
    [SerializeField] private int m_TargetNumber = 1;
    [SerializeField] private bool m_RequireInsideCollider;
    [SerializeField] private UnityEvent m_StartBulletEvent;
    [SerializeField] private UnityEvent m_MaxDistanceEvent;
    [SerializeField] private UnityEvent m_HitEvent;
    [SerializeField] private UnityEvent<Actor> m_HitTargetEvent;
    [SerializeField] private UnityEvent m_DespawnEvent;

    private int m_TargetCount;
    private Actor m_Owner;
    private bool m_Update;
    private float m_Timer;
    [SerializeField, ReadOnly] private float m_DebugDistance;

    public Transform Trans { get; private set; }

    public LayerMask TargetLayer
    {
        set { m_TargetLayer = value; }
        get { return m_TargetLayer; }
    }

    public float MaxDistance
    {
        set { m_MaxDistance = value; }
        get { return m_MaxDistance; }
    }

    public bool ReachMaxDistance { set; get; }
    [ShowInInspector, ReadOnly] public Transform Target { set; get; }
    [ShowInInspector, ReadOnly] public Vector3 TargetPosition { set; get; }

    public bool DestroyOnMaximumDistance
    {
        set { m_DestroyOnMaximumDistance = value; }
        get { return m_DestroyOnMaximumDistance; }
    }

    public bool DestroyOnImpact
    {
        set { m_DestroyOnImpact = value; }
        get { return m_DestroyOnImpact; }
    }

    public DamageDealer DamageDealer
    {
        set { m_DamageDealer = value; }
        get { return m_DamageDealer; }
    }

    public Vector3 StartingPosition { get; private set; } = Vector3.zero;
    public IBulletMovement Movement { get; private set; }

    public UnityEvent StartBulletEvent => m_StartBulletEvent;
    public UnityEvent MaxDistanceEvent => m_MaxDistanceEvent;
    public UnityEvent HitEvent => m_HitEvent;
    public UnityEvent DespawnEvent => m_DespawnEvent;
    public Action<Bullet2D, Actor> OnHitTarget;

    [ShowInInspector, ReadOnly]
    public Actor Owner
    {
        set
        {
            m_Owner = value;
            if (m_DamageDealer != null)
            {
                m_DamageDealer.Owner = value;
            }
        }
        get { return m_Owner; }
    }

    protected virtual void Awake()
    {
        Trans = transform;
        Movement = GetComponent<IBulletMovement>();
    }

    protected virtual void Update()
    {
        if (!m_Update) return;

        if (m_DestroyOnMaximumDuration)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= m_MaxDuration)
            {
                Despawn();
            }
        }

        if (ReachMaxDistance) return;
        float sqrDist = Vector3.SqrMagnitude(Trans.position - StartingPosition);
        m_DebugDistance = sqrDist;

        if (!(sqrDist >= m_MaxDistance * m_MaxDistance)) return;

        ReachMaxDistance = true;
        m_MaxDistanceEvent.Invoke();

        if (m_DestroyOnMaximumDistance)
        {
            Despawn();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        var targetPosition = other.transform.position;

        if (m_RequireInsideCollider && (m_Collider == null || !m_Collider.OverlapPoint(targetPosition)))
        {
            return;
        }

        if (m_TargetLayer.Contains(other.gameObject) && m_TargetCount < m_TargetNumber)
        {
            var target = other.GetComponent<Actor>();
            if (target == null) return;
            if (m_DealDamageOnContact && m_DamageDealer != null)
            {
                m_TargetCount++;
                var hitResult = m_DamageDealer.DealDamage(Owner, target);

                if (hitResult.Success)
                {
                    OnImpact(target);

                    if (hitResult.Critical)
                    {
                        if (m_CritHitEffect != null)
                        {
                        }
                    }
                    else
                    {
                        if (m_ImpactEffect != null)
                        {
                            PoolManager.Instance.Spawn(m_ImpactEffect, target.CenterPosition, Quaternion.identity);
                        }
                    }

                    m_HitTargetEvent.Invoke(target);
                }
            }

            m_HitEvent.Invoke();
            OnHitTarget?.Invoke(this, target);

            if (m_DestroyOnImpact)
            {
                Despawn();
            }
        }
    }

    public virtual void StartBullet()
    {
        m_Timer = 0;
        m_TargetCount = 0;
        StartingPosition = Trans.position;
        ReachMaxDistance = false;

        m_StartBulletEvent.Invoke();

        if (m_AppearEffect != null)
        {
            PoolManager.Instance.Spawn(m_AppearEffect, StartingPosition, Quaternion.identity);
        }

        Movement?.Move();

        m_Update = true;
    }

    protected virtual void OnImpact(Actor target)
    {
    }

    public virtual void CreateImpactEffect()
    {
        if (m_ImpactEffect != null)
        {
            PoolManager.Instance.Spawn(m_ImpactEffect, Trans.position, Quaternion.identity);
        }
    }

    public virtual void Reset()
    {
        m_Update = false;
        m_TargetCount = 0;
        ReachMaxDistance = false;
        Target = null;
        TargetPosition = Vector3.zero;
        OnHitTarget = null;
        Movement?.Reset();
    }

    public virtual void Despawn()
    {
        Reset();
        PoolManager.Instance.Despawn(gameObject);
        m_DespawnEvent?.Invoke();
    }
}