using Game.GameActor;
using UnityEngine;

public sealed class AttackCircleCast2D
{
    private readonly RaycastHit2D[] _hits;
    private readonly ActorBase _actor;

    public delegate void HitSuccessDelegate(ActorBase target);


    public HitSuccessDelegate OnSuccess;

    public AttackCircleCast2D(ActorBase actor, int maxTargetNumber)
    {
        _hits = new RaycastHit2D[maxTargetNumber];
        _actor = actor;
    }

    public void DealDamage(Vector3 start, Vector3 direction, float radius, LayerMask mask)
    {



        _hits.CleanUp();

        int count = Physics2D.CircleCastNonAlloc(start, radius, direction, _hits, radius, mask);

        if (count <= 0) return;

        foreach (var hit in _hits)
        {
            if (hit == default(RaycastHit2D)) continue;
            var target = hit.collider.GetComponentInParent<ActorBase>();

            if (target == null) continue;

            OnSuccess?.Invoke(target);
        }
    }
}