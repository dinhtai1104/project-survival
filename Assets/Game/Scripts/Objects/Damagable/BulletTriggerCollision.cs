using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Engine;

public class BulletTriggerCollision : MonoBehaviour
{
    [SerializeField] private LayerMask m_TargetLayerMask;

    [SerializeField] private DamageDealer m_DamageDealer;

    [SerializeField] private int m_TargetNumber = 5;
    [SerializeField] private bool m_DealDamageOnCircle;
    [SerializeField] private bool m_DealDamageOnArea;

    [SerializeField, Range(0f, 10f), ShowIf("@m_DealDamageOnCircle && !m_DealDamageOnArea")]
    private float m_Radius;

    [SerializeField, ShowIf("@!m_DealDamageOnCircle && m_DealDamageOnArea")]
    private Vector3 m_Point1;

    [SerializeField, ShowIf("@!m_DealDamageOnCircle && m_DealDamageOnArea")]
    private Vector3 m_Point2;

    [SerializeField] private bool m_Shake = true;

    private Bullet2D m_Bullet;
    private AttackOverlapCircle m_AttackOverlapCircle;
    private AttackOverlapArea m_AttackOverlapArea;
    private Transform m_Trans;

    private void Awake()
    {
        m_Bullet = GetComponent<Bullet2D>();
        m_Trans = transform;
    }

    public void OnTrigger(ActorBase actor)
    {
        var attacker = m_Bullet.Owner;
        m_DamageDealer.Init(attacker.Stats);

        if (m_DealDamageOnCircle)
        {
            m_AttackOverlapCircle = new AttackOverlapCircle(attacker, m_DamageDealer, m_TargetNumber);
            m_AttackOverlapCircle.DealDamage(actor.Trans.position, m_Radius, m_TargetLayerMask);
        }

        if (m_DealDamageOnArea)
        {
            m_AttackOverlapArea = new AttackOverlapArea(attacker, m_DamageDealer, m_TargetNumber);
            m_AttackOverlapArea.DealDamage(actor.Trans.position + m_Point1, actor.Trans.position + m_Point2, m_TargetLayerMask);
        }

        if (m_Shake)
        {
        }
    }

    public void OnTrigger()
    {
        var attacker = m_Bullet.Owner;
        m_DamageDealer.Init(attacker.Stats);

        if (m_DealDamageOnCircle)
        {
            m_AttackOverlapCircle = new AttackOverlapCircle(attacker, m_DamageDealer, m_TargetNumber);
            m_AttackOverlapCircle.DealDamage(m_Trans.position, m_Radius, m_TargetLayerMask);
        }

        if (m_DealDamageOnArea)
        {
            m_AttackOverlapArea = new AttackOverlapArea(attacker, m_DamageDealer, m_TargetNumber);
            m_AttackOverlapArea.DealDamage(m_Trans.position + m_Point1, m_Trans.position + m_Point2, m_TargetLayerMask);
        }

        if (m_Shake)
        {
            // Shake
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        if (m_DealDamageOnCircle)
        {
            DebugExtension.DrawCircle(transform.position, Vector3.forward, Color.red, m_Radius);
        }

        if (m_DealDamageOnArea)
        {
            var center = (m_Point1 + m_Point2) / 2f;
            Vector3 size = new Vector3(Mathf.Abs(m_Point1.x - m_Point2.x), Mathf.Abs(m_Point1.y - m_Point2.y));
            DebugExtension.DebugBounds(new Bounds(center, size), Color.red);
        }
    }
}