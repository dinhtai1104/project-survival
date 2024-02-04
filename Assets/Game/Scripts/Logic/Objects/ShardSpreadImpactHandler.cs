using Cysharp.Threading.Tasks;
using Game.Effect;
using Game.GameActor;
using Game.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ShardSpreadImpactHandler : ImpactHandler
{
 
    [SerializeField]
    private ValueConfigSearch BulletVelocity;

    [SerializeField]
    private AssetReferenceGameObject bulletRef;

    [SerializeField]
    private List< ShootPatternBase> shootPatterns;
    private List< ShootPatternBase> shootPatternInstances;


    
    public override void SetUp(BulletBase bulletBase)
    {
        base.SetUp(bulletBase);
        SetUpPattern();

        async UniTask SetUpPattern()
        {
            shootPatternInstances = new List<ShootPatternBase>();
            foreach (var pattern in shootPatterns)
            {
                shootPatternInstances.Add(await pattern.SetUp(Base.Caster));
            }
        }
    }
    
    public override void Impact(ITarget target)
    {
        if ((Object)target == Base.Caster || (target!=null && !Base.Caster.AttackHandler.targetType.Contains(target.GetCharacterType()))) return;

        if (!string.IsNullOrEmpty(impactEffect))
        {
            Vector3 pos = Base.GetTransform().position;
            GameObjectSpawner.Instance.Get(impactEffect, res =>
            {
                res.GetComponent<EffectAbstract>().Active(pos);
            });
        }

        gameObject.SetActive(false);
        TriggerPattern();
    }

    List<UniTask> patternTasks = new List<UniTask>();
    async UniTask TriggerPattern()
    {
        patternTasks.Clear();
        int index = 0;
        foreach (var pattern in shootPatternInstances)
        {
            patternTasks.Add(pattern.Trigger(Base.weaponBase, Base.GetTransform(), BulletVelocity.FloatValue,null, Base.Caster.GetLookDirection(), Base.Caster.Sensor.CurrentTarget,bulletRef, onShot: () =>
            {
                //PlayShotSFX();
                //PlayTriggerImpact();
            }));
            index++;
        }
        await UniTask.WhenAll(patternTasks);


    }

}