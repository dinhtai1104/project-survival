using System;
using System.Collections;
using System.Collections.Generic;
using RVO;
using UnityEngine;
using Random = System.Random;
using Vector2RVO = RVO.Vector2RVO;

public class GameAgent : MonoBehaviour
{
    [HideInInspector] public int sid = -1;

    /** Random number generator. */
    private Random m_random = new Random();
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (sid >= 0)
        {
            Vector2RVO pos = Simulator.Instance.getAgentPosition(sid);
            Vector2RVO vel = Simulator.Instance.getAgentPrefVelocity(sid);
            transform.position = new Vector3(pos.x(), transform.position.y, pos.y());
            if (Math.Abs(vel.x()) > 0.01f && Math.Abs(vel.y()) > 0.01f)
                transform.forward = new Vector3(vel.x(), 0, vel.y()).normalized;
        }

        if (!Input.GetMouseButton(1))
        {
            Simulator.Instance.setAgentPrefVelocity(sid, new Vector2RVO(0, 0));
            return;
        }

        Vector2RVO goalVector = GameMainManager.Instance.mousePosition - Simulator.Instance.getAgentPosition(sid);
        if (RVOMath.absSq(goalVector) > 1.0f)
        {
            goalVector = RVOMath.normalize(goalVector);
        }

        Simulator.Instance.setAgentPrefVelocity(sid, goalVector);

        /* Perturb a little to avoid deadlocks due to perfect symmetry. */
        float angle = (float) m_random.NextDouble()*2.0f*(float) Math.PI;
        float dist = (float) m_random.NextDouble()*0.0001f;

        Simulator.Instance.setAgentPrefVelocity(sid, Simulator.Instance.getAgentPrefVelocity(sid) +
                                                     dist*
                                                     new Vector2RVO((float) Math.Cos(angle), (float) Math.Sin(angle)));
    }
}