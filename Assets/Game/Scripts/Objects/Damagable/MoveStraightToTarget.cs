using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

public class MoveStraightToTarget : MonoBehaviour, IBulletMovement
{
    [SerializeField] private Stat m_SpeedStat = new Stat(10);
    [SerializeField] private bool m_LookAt;
    [SerializeField, Range(0f, 180f)] private float m_NoiseAngle;

    private Bullet2D m_Bullet;
    private Transform m_Trans;
    private Vector3 m_Dir;
    private Vector3 m_TargetPos;
    private bool m_Update;

    public Stat Speed { get { return m_SpeedStat; } set { m_SpeedStat = value; } }

    private void Awake()
    {
        m_Trans = transform;
        m_Bullet = GetComponent<Bullet2D>();
    }

    public void Move()
    {
        Speed.RecalculateValue();
        var target = m_Bullet.Owner.TargetFinder.CurrentTarget;
        if (target != null)
        {
            m_Dir = Vector3.Normalize(target.Trans.position - m_Trans.position);
            m_TargetPos = target.Trans.position;
        }
        else
        {
            m_Dir = Vector3.Normalize(m_Bullet.TargetPosition - m_Trans.position);
            m_TargetPos = m_Bullet.TargetPosition;
        }

        m_Dir.z = 0f;
        m_TargetPos.z = 0f;

        if (m_NoiseAngle > 0f)
        {
            AddNoiseDirectionVector(m_Dir, Vector3.forward, Random.Range(-m_NoiseAngle, m_NoiseAngle));
        }

        if (m_LookAt) m_Trans.LookAt2D(m_TargetPos);

        m_Update = true;
    }


    private void Update()
    {
        m_Trans.Translate(m_Dir * (Time.deltaTime * m_SpeedStat.Value), Space.World);
    }

    public GameObject ModSource => m_Bullet.Owner == null ? gameObject : m_Bullet.Owner.gameObject;

    public void AddSpeedModifier(StatModifier modifier)
    {
        m_SpeedStat.AddModifier(modifier);
    }

    public void RemoveSpeedModifier(StatModifier modifier)
    {
        m_SpeedStat.RemoveModifier(modifier);
    }

    public void Reset()
    {
        m_Update = false;
        m_SpeedStat.ClearModifiers();
        m_SpeedStat.ClearListeners();
    }

    private Vector3 AddNoiseDirectionVector(Vector3 v, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * v;
    }
}