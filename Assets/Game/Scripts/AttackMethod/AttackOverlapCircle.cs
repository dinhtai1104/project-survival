using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AttackOverlapCircle
{
    private Actor m_Actor;
    private Collider2D[] m_Hits;
    private IDamageDealer m_DamageDealer;

    public delegate void HitSuccessDelegate(Actor target, Vector3 hitPoint, bool critical, bool lastHit);

    public delegate void HitFailedDelegate(Actor target);

    public HitSuccessDelegate OnSuccess;
    public HitFailedDelegate OnFailed;

    public AttackOverlapCircle(Actor actor, IDamageDealer damageDealer, int maxTargetNumber)
    {
        m_Actor = actor;
        m_DamageDealer = damageDealer;
        m_Hits = new Collider2D[maxTargetNumber];
    }

    public void SetAttacker(Actor attacker)
    {
        m_Actor = attacker;
    }

    public void DealDamage(Vector3 center, float radius, LayerMask mask)
    {
#if UNITY_EDITOR
        DebugExtension.DebugCircle(center, Vector3.forward, Color.red, radius, 0.6f);
#endif

        m_Hits.CleanUp();

        int count = Physics2D.OverlapCircleNonAlloc(center, radius, m_Hits, mask);
        if (count <= 0) return;

        foreach (var hit in m_Hits)
        {
            if (hit == null) continue;
            var target = hit.GetComponent<Actor>();

            if (target == null) continue;

            HitResult hitResult = m_DamageDealer.DealDamage(m_Actor, target);

            if (hitResult.Success)
            {
                OnSuccess?.Invoke(target, target.GraphicTrans.position, hitResult.Critical, hitResult.LastHit);
            }
            else
            {
                OnFailed?.Invoke(target);
            }
        }
    }
}