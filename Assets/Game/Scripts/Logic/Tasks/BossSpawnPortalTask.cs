using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BossSpawnPortalTask : SkillTask
{
    [SerializeField] private AssetReferenceGameObject portalRef;
    [SerializeField] private Vector3 offset;
    public override async UniTask Begin()
    {
        Game.Pool.GameObjectSpawner.Instance.Get(portalRef.RuntimeKey.ToString(), obj =>
        {
            obj.transform.position = Caster.WeaponHandler.GetCurrentAttackPoint().position +offset;
        });
        await base.Begin();

        IsCompleted = true;
    }
    
}
