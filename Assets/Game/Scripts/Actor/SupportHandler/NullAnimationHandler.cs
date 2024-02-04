using Game.GameActor;
using Spine.Unity;
using UnityEngine;

public class NullAnimationHandler : IAnimationHandler
{
    public MeshRenderer MeshRenderer => null;

    public event AnimationHandler.OnTriggerComplete onCompleteTracking;
    public event AnimationHandler.OnTriggerEventTrack onEventTracking;
    public event AnimationHandler.OnTriggerStart onStartTracking;
    public event AnimationHandler.OnTriggerEvent onTriggerEvent;

    public void AddAnimation(int index, string animationName, bool isLoop, float delay = 0)
    {
    }

    public void Clear()
    {
    }

    public void ClearTracks()
    {
    }

    public SkeletonAnimation GetAnimator()
    {
        return null;
    }

    public float GetCurrentTrackTime()
    {
        return 0;
    }

    public string GetSkin()
    {
        return "";
    }

    public Transform GetTransform()
    {
        return null;
    }

    public void OnDestroy()
    {
    }

    public void OnDisable()
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void PlayAtTime(string animation, float time)
    {
    }

    public void SetAnimation(int track, string animationName, bool isLoop)
    {
    }

    public void SetAnimation(string animationName, bool isLoop)
    {
    }

    public void SetAttackIdle()
    {
    }

    public void SetClimb()
    {
    }

    public void SetDead()
    {
    }

    public void SetGetHit()
    {
    }

    public void SetIdle()
    {
    }

    public void SetJump(int jumpIndex)
    {
    }

    public void SetJumpDown()
    {
    }

    public void SetLand()
    {
    }

    public void SetLandIdle()
    {
    }

    public void SetRun()
    {
    }

    public void SetShoot()
    {
    }

    public void SetSkin(string skin)
    {
    }

    public void SetTimeScale(float scale)
    {
    }

    public void SetWin()
    {
    }
}
