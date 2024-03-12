using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorAnimationEngine : MonoBehaviour, IAnimationEngine
    {
        private Animator m_Animator;
        private Actor m_Owner;
        private float m_AnimationTimeScale = 1.0f;
        private float m_PrevAnimationTimeScale = 0.0f;
        private bool m_IsAnimationPaused = false;
        public Actor Owner => m_Owner;

        public bool Lock { get; set; }

        public string CurrentAnimationName => m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        public bool IsCurrentAnimationComplete
        {
            get
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && m_Animator.IsInTransition(0))
                {
                    return true;
                }
                return false;
            }
        }

        public float FlipX { set; get; }
        public float FlipY { set; get; }
        public float TimeScale
        {
            set
            {
                m_PrevAnimationTimeScale = m_AnimationTimeScale;
                m_AnimationTimeScale = value;
                m_Animator.speed = value;
            }
            get
            {
                return m_AnimationTimeScale;
            }
        }

        public void Init(Actor actor)
        {
            m_Owner = actor;
        }

        public void AddAnimation(int track, string animName, bool loop = false, float delay = 0)
        {
        }

        public void Clear()
        {
        }

        public void ClearTrack(int track)
        {
        }

        public bool EnsurePlay(int track, string animName, bool loop = true, bool restart = false)
        {
            if (IsPlaying(animName)) return false;
            m_Animator.Play(animName);
            return true;
        }

        public Spine.Animation FindAnimation(string animName)
        {
            return null;
        }

        public Bone FindBone(string boneName)
        {
            return null;
        }

        public EventData FindEvent(string eventName)
        {
            return null;
        }

        public bool HasAnimation(string animName)
        {
            foreach (AnimationClip clip in m_Animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animName)
                {
                    return true;
                }
            }
            return false;
        }


        public bool IsPlaying(string animName)
        {
            return false;
        }

        public bool IsPlaying(int track, string animName)
        {
            return false;
        }

        public void Pause()
        {
            m_Animator.speed = 0;
        }

        public void Play(int track, string animName, bool loop = true, bool restart = false)
        {
            if (IsPlaying(animName)) return;
            m_Animator.Play(animName);
        }

        public void Resume()
        {
            m_Animator.speed = m_PrevAnimationTimeScale;
        }

        public void Stop()
        {
            m_Animator.speed = 0;
        }

        public void SubscribeComplete(Spine.AnimationState.TrackEntryDelegate callback)
        {
        }

        public void SubscribeEvent(Spine.AnimationState.TrackEntryEventDelegate callback)
        {
        }

        public void UnsubcribeComplete(Spine.AnimationState.TrackEntryDelegate callback)
        {
        }

        public void UnsubcribeEvent(Spine.AnimationState.TrackEntryEventDelegate callback)
        {
        }
    }
}
