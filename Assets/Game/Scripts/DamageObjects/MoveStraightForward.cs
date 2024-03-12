using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStraightForward : MonoBehaviour, IBulletMovement
{

    [SerializeField] private Stat m_SpeedStat = new Stat(10);

    private Transform m_Trans;
    private Bullet2D m_Bullet;
    private bool m_Update;

    public Stat Speed { get { return m_SpeedStat; } set { m_SpeedStat = value; } }

    private void Awake()
    {
        m_Trans = transform;
        m_Bullet = GetComponent<Bullet2D>();
    }

    private void Update()
    {
        if (!m_Update) return;
        m_Trans.Translate(m_Trans.right * (Time.deltaTime * m_SpeedStat.Value), Space.World);
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

    public void Move()
    {
        m_SpeedStat.RecalculateValue();
        m_Update = true;
    }

    public void Reset()
    {
        m_Update = false;
        m_SpeedStat.ClearModifiers();
        m_SpeedStat.ClearListeners();
    }
}