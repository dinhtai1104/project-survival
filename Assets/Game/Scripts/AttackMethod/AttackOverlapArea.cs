using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AttackOverlapArea
{
    private ActorBase m_Actor;
    private Collider2D[] m_Hits;
    private IDamageDealer m_DamageDealer;

    public delegate void HitSuccessDelegate(ActorBase target, Vector3 hitPoint, bool critical, bool lastHit);

    public delegate void HitFailedDelegate(ActorBase target);

    public HitSuccessDelegate OnSuccess;
    public HitFailedDelegate OnFailed;

    public AttackOverlapArea(ActorBase actor, IDamageDealer damageDealer, int maxTargetNumber)
    {
        m_Actor = actor;
        m_DamageDealer = damageDealer;
        m_Hits = new Collider2D[maxTargetNumber];
    }

    public void SetAttacker(ActorBase attacker)
    {
        m_Actor = attacker;
    }

    public void DealDamage(Vector3 p1, Vector3 p2, LayerMask mask)
    {
#if UNITY_EDITOR
        Vector3 center = (p1 + p2) / 2f;
        Vector3 size = new Vector3(Mathf.Abs(p1.x - p2.x), Mathf.Abs(p1.y - p2.y));
        DebugExtension.DebugBounds(new Bounds(center, size), Color.red, 0.6f);
#endif
        m_Hits.CleanUp();

        int count = Physics2D.OverlapAreaNonAlloc(p1, p2, m_Hits, mask);

        if (count <= 0) return;

        foreach (var hit in m_Hits)
        {
            if (hit == null) continue;
            var target = hit.GetComponent<ActorBase>();

            if (target == null) continue;

            HitResult hitResult = m_DamageDealer.DealDamage(m_Actor, target);

            if (hitResult.Success)
            {
                OnSuccess?.Invoke(target, target.GraphicTrans.position, hitResult.Critical,
                    hitResult.LastHit);
            }
            else
            {
                OnFailed?.Invoke(target);
            }
        }
    }
}