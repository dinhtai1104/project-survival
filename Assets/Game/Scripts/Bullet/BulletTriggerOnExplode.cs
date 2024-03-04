using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTriggerOnExplode : MonoBehaviour
{
    [SerializeField] private DamageDealer m_DamageDealer;
    [SerializeField] private int m_TargetNumber = 5;
    [SerializeField, Range(0f, 10f)] private float m_Radius;
    [SerializeField] private bool m_Shake = true;

    private Bullet2D m_Bullet;
    private AttackOverlapCircle m_AttackOverlap;
    private Transform m_Trans;

    private void Awake()
    {
        m_Bullet = GetComponent<Bullet2D>();
        m_Trans = transform;
    }

    public void OnTrigger()
    {
        var attacker = m_Bullet.Owner;
        m_DamageDealer.Init(attacker.Stat);
        m_AttackOverlap = new AttackOverlapCircle(attacker, m_DamageDealer, m_TargetNumber);
        m_AttackOverlap.DealDamage(m_Trans.position, m_Radius, m_Bullet.TargetLayer);
        if (m_Shake)
        {
        }
    }
}