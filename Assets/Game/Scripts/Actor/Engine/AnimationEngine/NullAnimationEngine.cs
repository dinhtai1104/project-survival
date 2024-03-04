using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using UnityEngine.Events;
using AnimationState = Spine.AnimationState;
using Event = Spine.Event;

namespace Engine
{
    public class NullAnimationEngine : IAnimationEngine
    {
        public Actor Owner { get; private set; }
        public bool Lock { get; set; }

        public string CurrentAnimationName
        {
            get { return string.Empty; }
        }

        public bool IsCurrentAnimationComplete
        {
            get { return false; }
        }

        public float FlipX
        {
            get { return 1; }
            set { }
        }

        public float FlipY
        {
            get { return 1; }
            set { }
        }

        public float TimeScale
        {
            get { return 1f; }
            set { }
        }

        public void SubscribeEvent(AnimationState.TrackEntryEventDelegate callback)
        {
        }

        public void SubscribeComplete(AnimationState.TrackEntryDelegate callback)
        {
        }

        public void UnsubcribeEvent(AnimationState.TrackEntryEventDelegate callback)
        {
        }

        public void UnsubcribeComplete(AnimationState.TrackEntryDelegate callback)
        {
        }


        public bool HasAnimation(string animName)
        {
            return false;
        }

        public bool IsPlaying(string animName)
        {
            return false;
        }

        public void Init(Actor actor)
        {
            Owner = actor;
        }

        public Spine.Animation FindAnimation(string animName)
        {
            return null;
        }

        public EventData FindEvent(string eventName)
        {
            return null;
        }

        public Bone FindBone(string boneName)
        {
            return null;
        }

        public void Play(int track, string animName, bool loop = true, bool restart = false)
        {
        }

        public bool EnsurePlay(int track, string animName, bool loop = true, bool restart = false)
        {
            return false;
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        public void Stop()
        {
        }

        public void SetAnimationTimeScale(float timeScale)
        {
        }

        public void Clear()
        {
        }
        
        public void ClearTrack(int track)
        {

        }

        public bool IsPlaying(int track, string animName)
        {
            return false;
        }

        public void AddAnimation(int track, string animName, bool loop = false, float delay = 0)
        {
        }
    }
}