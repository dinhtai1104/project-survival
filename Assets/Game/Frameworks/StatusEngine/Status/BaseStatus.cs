using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public abstract class BaseStatus : MonoBehaviour
{
    public EStatus Status;
    private float duration = 0;
    protected ActorBase sourceActor;
    protected ActorBase targetActor;
    protected object sourceCast;

    public ActorBase Source => sourceActor;
    public ActorBase Target => targetActor;
    public object SourceCast => sourceCast;

    public bool IsExpired => duration <= 0;

    public Game.Effect.EffectAbstract effect;

    public virtual void Init(ActorBase source, ActorBase target)
    {
        this.sourceActor = source;
        this.targetActor = target;

        effect = GetComponentInChildren<Game.Effect.EffectAbstract>(true);
        if (effect != null)
        {
            float scale = target.AnimationHandler.GetAnimator().skeletonDataAsset.scale;
            effect.Active(target.AnimationHandler.MeshRenderer, scale / 0.0015f);
        }
    }
    public void SetSourceCast(object sourceCast)
    {
        this.sourceCast = sourceCast;
    }

    public abstract void SetDmgMul(float dmgMul);
    public abstract void SetCooldown(float cooldown);
    public virtual void SetStatusEffect() { }
    public void SetDuration(float duration)
    {
        this.duration = duration;
    }

    public void Ticks(float deltaTime)
    {
        duration -= deltaTime;
        OnUpdate(deltaTime);
    }

    public abstract void OnUpdate(float deltaTime);
    protected virtual void Release()
    {
    }

    public virtual void Stop()
    {
        //Debug.Log("Release Status: " + gameObject.name + " on " + targetActor.gameObject.name);
        if (effect != null)
        {
            effect.Deactive();
        }
        Release();
        duration = 0;
        PoolManager.Instance.Despawn(gameObject);
    }
}