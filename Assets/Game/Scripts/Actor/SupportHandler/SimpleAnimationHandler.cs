using Game.GameActor;
using Spine.Unity;
using UnityEngine;

public class SimpleAnimationHandler : MonoBehaviour, IAnimationHandler
{
    public MeshRenderer MeshRenderer => Skeleton.GetComponent<MeshRenderer>();

    public event AnimationHandler.OnTriggerComplete onCompleteTracking;
    public event AnimationHandler.OnTriggerEventTrack onEventTracking;
    public event AnimationHandler.OnTriggerStart onStartTracking;
    public event AnimationHandler.OnTriggerEvent onTriggerEvent;
    private SkeletonAnimation skeleton;
    public SkeletonAnimation Skeleton => skeleton;

    protected virtual void Awake()
    {
        skeleton = GetComponent<SkeletonAnimation>();
        if (skeleton == null)
        {
            skeleton = GetComponentInChildren<SkeletonAnimation>();
        }
        if (skeleton == null)
        {
            skeleton = GetComponentInParent<SkeletonAnimation>();
        }
    }

    public virtual void AddAnimation(int index, string animationName, bool isLoop, float delay = 0)
    {
        Skeleton.AnimationState.AddAnimation(index, animationName, isLoop, delay);
    }

    public virtual void Clear()
    {
        Skeleton.ClearState();
    }

    public virtual void ClearTracks()
    {
        Skeleton.AnimationState.ClearTracks();
    }

    public virtual SkeletonAnimation GetAnimator()
    {
        return Skeleton;
    }

    public virtual float GetCurrentTrackTime()
    {
        return 0;
    }

    public virtual string GetSkin()
    {
        return "";
    }

    public virtual Transform GetTransform()
    {
        return transform;
    }

    public virtual void OnDestroy()
    {
        onCompleteTracking = null;
        onTriggerEvent = null;
        onEventTracking = null;
        onStartTracking = null;
    }

    public virtual void OnDisable()
    {
        onCompleteTracking = null;
        onTriggerEvent = null;
        onEventTracking = null;
        onStartTracking = null;
    }

    public virtual void OnUpdate(float deltaTime)
    {
    }

    public virtual void PlayAtTime(string animation, float time)
    {
    }

    public virtual void SetAnimation(int track, string animationName, bool isLoop)
    {
        Skeleton.AnimationState.SetAnimation(track, animationName, isLoop);
    }

    public virtual void SetAnimation(string animationName, bool isLoop)
    {
        Skeleton.AnimationState.SetAnimation(0, animationName, isLoop);
    }

    public virtual void SetAttackIdle()
    {
    }

    public virtual void SetClimb()
    {
    }

    public virtual void SetDead()
    {
    }

    public virtual void SetGetHit()
    {
    }

    public virtual void SetIdle()
    {
    }

    public virtual void SetJump(int jumpIndex)
    {
    }

    public virtual void SetJumpDown()
    {
    }

    public virtual void SetLand()
    {
    }

    public virtual void SetLandIdle()
    {
    }

    public virtual void SetRun()
    {
    }

    public virtual void SetShoot()
    {
    }

    public virtual void SetSkin(string skin)
    {
    }

    public virtual void SetTimeScale(float scale)
    {
    }

    public virtual void SetWin()
    {
    }
}
