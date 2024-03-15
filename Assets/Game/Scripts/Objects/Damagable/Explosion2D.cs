using Engine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Explosion2D : MonoBehaviour
{
    private ActorBase m_Owner;
    [SerializeField] private DamageDealer m_DamageDealer;
    [SerializeField] private LayerMask m_TargetLayer;
    [SerializeField] private UnityEvent m_StartExplosion;
    [SerializeField] private UnityEvent m_HitEvent;
    [SerializeField] private UnityEvent<ActorBase> m_HitTargetEvent;
    [SerializeField] private UnityEvent m_DespawnEvent;
    [SerializeField] private float m_Radius;
    public LayerMask TargetLayer
    {
        set { m_TargetLayer = value; }
        get { return m_TargetLayer; }
    }
    public DamageDealer DamageDealer
    {
        set { m_DamageDealer = value; }
        get { return m_DamageDealer; }
    }
    public float Radius
    {
        set 
        { 
            m_Radius = value;
            transform.localScale = Vector3.one * m_Radius;
        }
        get { return m_Radius; }
    }

    public UnityEvent HitEvent => m_HitEvent;
    public UnityEvent DespawnEvent => m_DespawnEvent;
    public Action<Bullet2D, ActorBase> OnHitTarget;
    [ShowInInspector, ReadOnly]
    public ActorBase Owner
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
    private AttackOverlapCircle m_AttackOverlapCircle;
    private void Awake()
    {
    }

    public void StartExplosion()
    {
        if (m_AttackOverlapCircle == null)
        {
            m_AttackOverlapCircle = new AttackOverlapCircle(Owner, m_DamageDealer, 20);
        }
        m_AttackOverlapCircle.DealDamage(transform.position, Radius, TargetLayer);
        m_StartExplosion?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
