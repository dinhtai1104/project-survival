using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AttackCircleCast2D
{
    private Actor m_Actor;
    private RaycastHit2D[] m_Hits;
    private IDamageDealer m_DamageDealer;

    public delegate void HitSuccessDelegate(Actor target, Vector3 hitPoint, bool critical, bool lastHit);

    public delegate void HitFailedDelegate(Actor target);

    public HitSuccessDelegate OnSuccess;
    public HitFailedDelegate OnFailed;

    public AttackCircleCast2D(Actor actor, IDamageDealer damageDealer, int maxTargetNumber)
    {
        m_Actor = actor;
        m_DamageDealer = damageDealer;
        m_Hits = new RaycastHit2D[maxTargetNumber];
    }

    public void SetAttacker(Actor attacker)
    {
        m_Actor = attacker;
    }

    public void DealDamage(Vector3 start, Vector3 direction, float radius, float range, LayerMask mask)
    {
#if UNITY_EDITOR
        Debug.DrawRay(start, direction * range, Color.red, 0.6f);
#endif

        m_Hits.CleanUp();

        int count = Physics2D.CircleCastNonAlloc(start, radius, direction, m_Hits, range, mask);

        if (count <= 0) return;

        foreach (var hit in m_Hits)
        {
            if (hit == default(RaycastHit2D)) continue;
            var target = hit.collider.GetComponent<Actor>();

            if (target == null) continue;

            HitResult hitResult = m_DamageDealer.DealDamage(m_Actor, target);

            if (hitResult.Success)
            {
                OnSuccess?.Invoke(target, hit.point, hitResult.Critical, hitResult.LastHit);
            }
            else
            {
                OnFailed?.Invoke(target);
            }
        }
    }
}