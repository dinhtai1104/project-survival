using com.mec;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CooldownToDieActionState : MonoBehaviour, IStateEnterCallback
{
    private ActorBase m_Actor;
    public ValueConfigSearch timelive;
    public void Action()
    {
        Timing.RunCoroutine(_CooldownDie(), gameObject);
    }
    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }

    private IEnumerator<float> _CooldownDie()
    {
        yield return Timing.WaitForSeconds(timelive.SetId(m_Actor.gameObject.name).FloatValue);
        if (!m_Actor.IsDead())
        {
            m_Actor.DeadForce();
        }
    }

    public void SetActor(ActorBase actor)
    {
        this.m_Actor = actor;
    }
}
