using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Engine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MoveToTargetPosition : MonoBehaviour, IBulletMovement
{
    [SerializeField] private bool m_IsSpeed = true;
    [SerializeField, ShowIf("m_IsSpeed")] private Stat m_Speed = new Stat(10);
    [SerializeField, HideIf("m_IsSpeed")] private float m_Duration;
    [SerializeField] private Ease m_Ease = Ease.Linear;
    [SerializeField] private float m_Delay = 0f;
    [SerializeField] private Vector3 m_TargetOffset;
    [SerializeField] private UnityEvent m_OnComplete;

    private Transform m_Trans;
    private Bullet2D m_Bullet;
    private Vector3 m_TargetPos;
    private Tweener m_Tween;

    public Stat Speed { get => m_Speed; set => m_Speed = value; }

    private void Awake()
    {
        m_Trans = transform;
        m_Bullet = GetComponent<Bullet2D>();
    }

    private void OnDisable()
    {
        Reset();
    }

    public void Move()
    {
        m_Speed.RecalculateValue();
        m_TargetPos = m_Bullet.TargetPosition + m_TargetOffset;
        m_Tween = m_Trans.DOMove(m_TargetPos, m_IsSpeed ? m_Speed.Value : m_Duration).SetSpeedBased(m_IsSpeed).SetEase(m_Ease).SetDelay(m_Delay).OnComplete(Arrived);
    }

    private void Arrived()
    {
        m_OnComplete.Invoke();
        m_Bullet.Despawn();
    }

    public void Reset()
    {
        m_Tween.Kill();
        m_Tween = null;
    }
}