using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class AddressableObjectSpawn : MonoBehaviour
{
    public Transform position;

    public abstract void Spawn();
    public abstract void DeSpawn();
}