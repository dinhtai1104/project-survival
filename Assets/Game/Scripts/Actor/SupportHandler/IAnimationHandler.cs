using Spine.Unity;
using UnityEngine;

namespace Game.GameActor
{
    public interface IAnimationHandler
    {
        MeshRenderer MeshRenderer { get; }

        event AnimationHandler.OnTriggerComplete onCompleteTracking;
        event AnimationHandler.OnTriggerEventTrack onEventTracking;
        event AnimationHandler.OnTriggerStart onStartTracking;
        event AnimationHandler.OnTriggerEvent onTriggerEvent;

        void AddAnimation(int index, string animationName, bool isLoop, float delay = 0);
        void Clear();
        void ClearTracks();
        SkeletonAnimation GetAnimator();
        float GetCurrentTrackTime();
        string GetSkin();
        Transform GetTransform();
        void OnDestroy();
        void OnDisable();
        void OnUpdate(float deltaTime);
        void PlayAtTime(string animation, float time);
        void SetAnimation(int track, string animationName, bool isLoop);
        void SetAnimation(string animationName, bool isLoop);
        void SetAttackIdle();
        void SetClimb();
        void SetDead();
        void SetGetHit();
        void SetIdle();
        void SetJump(int jumpIndex);
        void SetJumpDown();
        void SetLand();
        void SetLandIdle();
        void SetRun();
        void SetShoot();
        void SetSkin(string skin);
        void SetTimeScale(float scale);
        void SetWin();
    }
}