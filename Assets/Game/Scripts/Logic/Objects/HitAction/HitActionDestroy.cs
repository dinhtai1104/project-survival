using UnityEngine;

public class HitActionDestroy : MonoBehaviour, IHitTriggerAction
{
    public void Action(Collider2D collider)
    {
        PoolManager.Instance.Despawn(gameObject);
    }
}
