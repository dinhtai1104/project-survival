using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTask : Task
{
    [SerializeField, Range(0, 10f)] private float m_Duration;

    private float m_Timer;

    public override void Begin()
    {
        base.Begin();
        m_Timer = 0f;
    }

    public override void Run()
    {
        base.Run();
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_Duration)
        {
            m_Timer = 0f;
            IsCompleted = true;
        }
    }
}