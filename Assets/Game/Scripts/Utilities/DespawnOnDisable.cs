using Pool;
using UnityEngine;

public class DespawnOnDisable : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.Log("POOL:: Despawn " + gameObject.name);
        PoolManager.Instance.Despawn(gameObject);
    }
}