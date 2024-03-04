using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Engine;

public class BulletBoomerang : Bullet2D
{
    [SerializeField] private float m_ForwardDistance;
    [SerializeField] private float m_Speed;
    [SerializeField] private bool m_LookAt;

    [SerializeField, ReadOnly] private Vector3 m_TargetPos;
    [SerializeField, ReadOnly] private bool m_MoveForward;
    [SerializeField, ReadOnly] private Vector3 m_StartingPos;
    private Stat m_SpeedStat;

    protected override void Awake()
    {
        base.Awake();
        m_SpeedStat = new Stat(m_Speed, 0f, float.MaxValue);
    }

    public override void StartBullet()
    {
        base.StartBullet();
        m_StartingPos = StartingPosition;
        m_StartingPos.z = 0f;
        m_TargetPos = m_StartingPos;
        m_TargetPos += Trans.right * m_ForwardDistance;
        m_TargetPos.z = 0f;
        m_MoveForward = true;
    }

    protected override void Update()
    {
        base.Update();
        if (m_MoveForward)
        {
            var position = Vector3.MoveTowards(Trans.position, m_TargetPos, Time.deltaTime * m_SpeedStat.Value);
            position.z = 0f;

            if (Math.Abs(position.x - m_TargetPos.x) <= 0f)
            {
                m_MoveForward = false;
            }

            if (m_LookAt) Trans.LookAtAxisX2D(position);
            Trans.position = position;
        }
        else
        {
            var position = Vector3.MoveTowards(Trans.position, m_StartingPos, Time.deltaTime * m_SpeedStat.Value);
            position.z = 0f;

            if (m_LookAt) Trans.LookAtAxisX2D(position);
            Trans.position = position;

            if (Math.Abs(position.x - m_StartingPos.x) <= 0f)
            {
                Despawn();
            }
        }
    }
}