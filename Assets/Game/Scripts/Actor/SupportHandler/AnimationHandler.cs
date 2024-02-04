using Spine;
using System;
using UnityEngine;

namespace Game.GameActor
{
    public abstract class AnimationHandler : MonoBehaviour, IAnimationHandler
    {
        public delegate void OnTriggerEvent(Spine.Event e);
        public delegate void OnTriggerEventTrack(Spine.TrackEntry trackEntry, Spine.Event e);
        public delegate void OnTriggerComplete(Spine.TrackEntry trackEntry);
        public delegate void OnTriggerStart(Spine.TrackEntry trackEntry);

        public event OnTriggerEvent onTriggerEvent;
        public event OnTriggerEventTrack onEventTracking;
        public event OnTriggerComplete onCompleteTracking;
        public event OnTriggerStart onStartTracking;
        protected Character character;
        [SerializeField]
        protected Spine.Unity.SkeletonAnimation anim;
        private MeshRenderer meshRenderer;
        public MeshRenderer MeshRenderer { get { if (meshRenderer == null) meshRenderer = anim.GetComponent<MeshRenderer>(); return meshRenderer; } }

        public virtual void OnDisable()
        {
            if (anim != null)
            {
                anim.AnimationState.Event -= AnimationState_Event;
                anim.AnimationState.Event -= AnimationState_TrackEvent;

                anim.AnimationState.Complete -= AnimationState_Complete;
                anim.AnimationState.Start -= AnimationState_Start;
            }
            AnimationUpdateSlicer.Instance.DeregisterSlicedUpdate(this);
        }

        private void AnimationState_Start(TrackEntry trackEntry)
        {
            onStartTracking?.Invoke(trackEntry);
        }

        private void AnimationState_Complete(TrackEntry trackEntry)
        {
            onCompleteTracking?.Invoke(trackEntry);
        }

        public virtual void OnDestroy()
        {
            if (anim != null)
            {
                anim.AnimationState.Event -= AnimationState_Event;
                anim.AnimationState.Event -= AnimationState_TrackEvent;

                anim.AnimationState.Complete -= AnimationState_Complete;
                anim.AnimationState.Start -= AnimationState_Start;
            }
            AnimationUpdateSlicer.Instance.DeregisterSlicedUpdate(this);


        }


        private void OnEnable()
        {
            if (anim != null && anim.UpdateTiming.Equals(Spine.Unity.UpdateTiming.ManualUpdate))
            {
                AnimationUpdateSlicer.Instance.RegisterSlicedUpdate(this);
            }
        }
        public virtual void SetUp(Character character)
        {
            this.character = character;
            try
            {
                anim = character.GetComponentInChildren<Spine.Unity.SkeletonAnimation>(true);
                anim.AnimationState.Event -= AnimationState_Event;
                anim.AnimationState.Event -= AnimationState_TrackEvent;

                anim.AnimationState.Event += AnimationState_Event;
                anim.AnimationState.Event += AnimationState_TrackEvent;

                anim.AnimationState.Complete += AnimationState_Complete;
                anim.AnimationState.Start += AnimationState_Start;


                if (anim != null && anim.UpdateTiming.Equals(Spine.Unity.UpdateTiming.ManualUpdate))
                {
                    AnimationUpdateSlicer.Instance.RegisterSlicedUpdate(this);
                }
            }
            catch
            {

            }
        }
        public void OnUpdate(float deltaTime)
        {
            if (anim.UpdateTiming == Spine.Unity.UpdateTiming.ManualUpdate)
                anim.Update(deltaTime);
        }

        public void SetAnimation(string animationName, bool isLoop)
        {
            //  ClearTracks();
            anim.AnimationState.SetAnimation(0, animationName, isLoop);
        }
        public void SetAnimation(int track, string animationName, bool isLoop)
        {
            //  ClearTracks();
            anim.AnimationState.SetAnimation(track, animationName, isLoop);
            if (track != 0)
            {
                anim.AnimationState.AddEmptyAnimation(track, 0, 0);
            }
        }
        public void AddEmptyAnimation(int index)
        {
            anim.AnimationState.AddEmptyAnimation(index,0,0);
        }
        public void AddAnimation(int index, string animationName, bool isLoop, float delay = 0)
        {
            anim.AnimationState.AddAnimation(index, animationName, isLoop, delay);
        }

        private void AnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
        {
            onTriggerEvent?.Invoke(e);
        }
        private void AnimationState_TrackEvent(Spine.TrackEntry trackEntry, Spine.Event e)
        {
            onEventTracking?.Invoke(trackEntry, e);
        }

        public virtual void SetSkin(string skin)
        {
            if (anim != null)
            {
                anim.initialSkinName = skin;
                anim.Initialize(true);
            }
        }

        public Spine.Unity.SkeletonAnimation GetAnimator()
        {
            return anim;
        }
        public string GetSkin()
        {
            if (anim != null)
                return anim.Skeleton.Skin.Name;
            return string.Empty;
        }
        public virtual Transform GetTransform()
        {
            if (anim == null)
            {
                return character.GetTransform().GetChild(0);
            }
            return anim.transform;
        }
        public void Clear()
        {
            if (anim != null)
                anim.AnimationState.ClearTracks();
        }


        public abstract void SetLand();
        public abstract void SetLandIdle();
        public abstract void SetWin();
        public abstract void SetRun();
        public abstract void SetJump(int jumpIndex);
        public abstract void SetJumpDown();
        public abstract void SetClimb();
        public abstract void SetDead();
        public abstract void SetGetHit();
        public abstract void SetIdle();
        public abstract void SetShoot();



        public void PlayAtTime(string animation, float time)
        {
            if (anim == null) return;
            anim.AnimationState.SetAnimation(0, animation, true);
            anim.AnimationState.GetCurrent(0).TrackTime = time;
        }
        public float GetCurrentTrackTime()
        {
            if (anim != null)
                return anim.AnimationState.GetCurrent(0).TrackTime;
            return 0;
        }
        public void ClearTracks()
        {
            if (anim != null)
            {
                anim.AnimationState.ClearTracks();
                anim.ClearState();
            }
        }
        public abstract void SetAttackIdle();

        public virtual void SetTimeScale(float scale)
        {
            anim.timeScale = scale;
        }
    }
}