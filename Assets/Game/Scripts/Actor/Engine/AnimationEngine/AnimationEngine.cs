using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using AnimationState = Spine.AnimationState;
using Event = Spine.Event;

namespace Engine
{
    public class AnimationEngine : MonoBehaviour, IAnimationEngine
    {
        [SerializeField] private SkeletonAnimation m_Animation;
        [SerializeField, ReadOnly] private string m_CurrentAnimation;

        private float m_FlipX;
        private float m_FlipY;
        private float m_AnimationTimeScale = 1.0f;
        private float m_PrevAnimationTimeScale = 0.0f;
        private bool m_IsAnimationPaused = false;

        public Actor Owner { get; private set; }
        public bool Lock { get; set; }

        #region MonoBehaviour Method

        public void Init(Actor actor)
        {
            Owner = actor;
            m_Animation = GetComponentInChildren<SkeletonAnimation>();
        }

        public Spine.Animation FindAnimation(string animName)
        {
            return m_Animation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(animName);
        }

        public EventData FindEvent(string eventName)
        {
            return m_Animation.Skeleton.Data.FindEvent(eventName);
        }

        public Bone FindBone(string boneName)
        {
            return m_Animation.Skeleton.FindBone(boneName);
        }

        public void Clear()
        {
            m_Animation.state.ClearTracks();
            m_Animation.Skeleton.SetToSetupPose();
        }

        #endregion

        public string CurrentAnimationName
        {
            get
            {
                var currentTrack = m_Animation.AnimationState.GetCurrent(0);
                return currentTrack != null ? currentTrack.Animation.Name : string.Empty;
            }
        }

        public bool IsCurrentAnimationComplete
        {
            get
            {
                var currentTrack = m_Animation.AnimationState.GetCurrent(0);
                return currentTrack == null || currentTrack.IsComplete;
            }
        }

        public float FlipX
        {
            get { return m_FlipX; }
            set { m_FlipX = value; }
        }

        public float FlipY
        {
            get { return m_FlipY; }
            set { m_FlipY = value; }
        }

        public float TimeScale
        {
            set => m_Animation.timeScale = value;
            get => m_Animation.timeScale;
        }

#if UNITY_EDITOR
        private void Update()
        {
            m_CurrentAnimation = CurrentAnimationName;
        }
#endif

        public void SubscribeEvent(AnimationState.TrackEntryEventDelegate callback)
        {
            m_Animation.AnimationState.Event += callback;
        }

        public void SubscribeComplete(AnimationState.TrackEntryDelegate callback)
        {
            m_Animation.AnimationState.Complete += callback;
        }

        public void UnsubcribeEvent(AnimationState.TrackEntryEventDelegate callback)
        {
            m_Animation.AnimationState.Event -= callback;
        }

        public void UnsubcribeComplete(AnimationState.TrackEntryDelegate callback)
        {
            m_Animation.AnimationState.Complete -= callback;
        }

        public bool HasAnimation(string animName)
        {
            if (m_Animation == null || m_Animation.Skeleton == null) return false;
            for (int i = 0; i < m_Animation.Skeleton.Data.Animations.Count; i++)
            {
                if (animName == m_Animation.Skeleton.Data.Animations.Items[i].Name)
                {
                    return true;
                }
            }

            Debug.Log("Animation was not found - " + animName);
            return false;
        }

        public bool IsPlaying(string animName)
        {
            if (m_Animation == null) return false;
            if (m_Animation.AnimationState?.GetCurrent(0) == null) return false;
            return string.Compare(m_Animation.AnimationState.GetCurrent(0).Animation.Name, animName,
                StringComparison.Ordinal) == 0;
        }

        public bool IsPlaying(int track, string animName)
        {
            if (m_Animation == null) return false;
            if (m_Animation.AnimationState?.GetCurrent(track) == null) return false;
            return string.Compare(m_Animation.AnimationState.GetCurrent(track).Animation.Name, animName,
                StringComparison.Ordinal) == 0;
        }

        public void Play(int track, string animName, bool loop = true, bool restart = false)
        {
            if (Lock) return;
            if (m_Animation.AnimationState == null || !HasAnimation(animName)) return;
            if (m_Animation.AnimationState == null || !restart && IsPlaying(track, animName)) return;
            var mix = m_Animation.AnimationState.SetAnimation(track, animName, loop);
            if (restart) mix.MixDuration = 0;
        }

        public bool EnsurePlay(int track, string animName, bool loop = true, bool restart = false)
        {
            if (Lock) return false;
            if (m_Animation.AnimationState == null || !HasAnimation(animName)) return false;
            if (!restart && IsPlaying(track, animName)) return true;
            var mix = m_Animation.AnimationState.SetAnimation(track, animName, loop);
            if (restart) mix.MixDuration = 0;
            return false;
        }

        public void Pause()
        {
            if (!m_IsAnimationPaused)
            {
                m_IsAnimationPaused = true;
                m_PrevAnimationTimeScale = m_AnimationTimeScale;
                m_AnimationTimeScale = 0.0f;
            }
        }

        public void Resume()
        {
            if (m_IsAnimationPaused)
            {
                m_IsAnimationPaused = false;
                m_AnimationTimeScale = m_PrevAnimationTimeScale;
            }
        }

        public void Stop()
        {
            m_Animation.state.ClearTracks();
            m_Animation.Skeleton.SetToSetupPose();
        }

        public void SetAnimationTimeScale(float timeScale)
        {
            m_AnimationTimeScale = timeScale;
            m_IsAnimationPaused = false;
        }

        public void ClearTrack(int track)
        {
            m_Animation.AnimationState.ClearTrack(track);
        }

        public void AddAnimation(int track, string animName, bool loop = false, float delay = 0)
        {
            if (Lock) return;
            if (m_Animation.AnimationState == null || !HasAnimation(animName)) return;
            if (IsPlaying(track, animName)) return;
            m_Animation.AnimationState.AddAnimation(track, animName, loop, delay);
            return;
        }
    }
}