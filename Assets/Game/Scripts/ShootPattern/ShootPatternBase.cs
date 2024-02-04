using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum EPatternStat
{
    Gravity
}
public abstract class ShootPatternBase : ScriptableObject
{
    public List<ProjectileEmitter> emitters=new List<ProjectileEmitter>();
    public Dictionary<EPatternStat, float> stats=new Dictionary<EPatternStat, float>();
    public virtual async UniTask<ShootPatternBase> SetUp(ActorBase actor)
    {
        ShootPatternBase instance = (ShootPatternBase)CreateInstance(this.GetType().ToString());
        instance.Init(actor);
        instance.stats = stats;
        return instance;
    }

    public void AddStat(EPatternStat statKey,float value)
    {
        if (!stats.ContainsKey(statKey))
        {
            stats.Add(statKey, value);
        }
        else
        {
            stats[statKey] = value;
        }
    }
    public float GetStat(EPatternStat statKey,float defaultValue = 0)
    {
        if (!stats.ContainsKey(statKey))
        {
            return defaultValue;
        }
        return stats[statKey];
    }

    public abstract void Init(ActorBase actor);
    public abstract UniTask Trigger(WeaponBase weapon,Transform triggerPos,float bulletVelocity, Transform target, Vector2 facing, ITarget trackingTarget, AssetReferenceGameObject bulletRef, System.Action onShot);
    //public abstract UniTask Trigger(WeaponBase weapon,Transform triggerPos, float bulletVelocity, Transform target, ITarget trackingTarget,  AssetReferenceGameObject bulletRef, System.Action onShot);
    public abstract UniTask Release();
    public abstract void Destroy();

}
