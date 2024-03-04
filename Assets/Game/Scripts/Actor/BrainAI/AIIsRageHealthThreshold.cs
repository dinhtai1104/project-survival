using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_IsRageHealthThreshold.asset", menuName = SOUtility.GAME_AI + "AI_IsRageHealthThreshold")]
public class AIIsRageHealthThreshold : BrainDecision
{
    [SerializeField, Range(0f, 1f)] private float m_RageHealthThreshold = 1.0f;

    public override bool Decide(Actor actor)
    {
        return actor.Health.HealthPercentage <= m_RageHealthThreshold;
    }
}