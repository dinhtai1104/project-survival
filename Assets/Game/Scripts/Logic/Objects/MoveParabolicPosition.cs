using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MoveParabolicPosition : MonoBehaviour
{
    [SerializeField, LabelText("Speed")] private Stat m_Speed;
    [SerializeField] private float m_ArcHeight = 1;
    private float m_MinThreshold = 0.2f;
    [SerializeField] private Vector3 m_TargetOffset;

    private Transform m_Trans;
    private Vector3 m_StartPos;
    private Vector3 m_TargetPos;
    private bool m_ReachTarget;

    private void Awake()
    {
        m_Trans = transform;
    }

    public void Move(Stat speed, Vector3 targetPos)
    {
        m_ReachTarget = false;
        m_StartPos = m_Trans.position;
        m_TargetPos = targetPos;

        // m_TargetPos = (m_Bullet.Target != null ? m_Bullet.Target.position : m_Bullet.TargetPosition) + m_TargetOffset;
        //m_Distance = Mathf.Abs(m_TargetPos.x - m_StartPos.x);
        m_Speed = speed;
        m_Speed.BaseValue = speed.Value;
    }

    private void Update()
    {
        if (!m_ReachTarget)
        {
            // var targetPos = m_Bullet.Target != null ? m_Bullet.Target.position : m_TargetPos;
            var x0 = m_StartPos.x;
            var x1 = m_TargetPos.x;
            var dist = x1 - x0;

            var position = m_Trans.position;
            var maxDelta = m_Speed.Value * Time.deltaTime;

            var nextX = Mathf.MoveTowards(position.x, x1, maxDelta);
            var t = (nextX - x0) / dist;
            var baseY = Mathf.Lerp(m_StartPos.y, m_TargetPos.y, t);
            var arc = m_ArcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            var nextPos = new Vector3(nextX, baseY + arc, position.z);

            m_Trans.rotation = LookAt2D(nextPos - position);
            m_Trans.position = nextPos;

            if (Vector3.Distance(nextPos, m_TargetPos) <= m_MinThreshold)
            {
                Arrived();
            }
        }
    }

    public void Reset()
    {
        m_ReachTarget = false;
    }

    private void Arrived()
    {
        m_ReachTarget = true;
        //PoolManager.Instance.Despawn(gameObject);
    }

    private Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
}
