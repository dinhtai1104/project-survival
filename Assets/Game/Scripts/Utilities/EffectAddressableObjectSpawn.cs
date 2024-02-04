using Game.Pool;
using UnityEngine.AddressableAssets;

public class EffectAddressableObjectSpawn : AddressableObjectSpawn
{
    public NormalEffect effectPrefab;
    private NormalEffect effect;
    public override void DeSpawn()
    {
        PoolManager.Instance.Despawn(effect.gameObject);
    }

    public override void Spawn()
    {
        effect = PoolManager.Instance.Spawn(effectPrefab);
        effect.transform.position = position.position;
        effect.Active();
    }
}