using Spine;

namespace Engine
{
    public interface IAnimationEngine
    {
        ActorBase Owner { get; }
        bool Lock { set; get; }
        string CurrentAnimationName { get; }
        bool IsCurrentAnimationComplete { get; }
        float FlipX { set; get; }
        float FlipY { set; get; }
        float TimeScale { set; get; }
        void SubscribeEvent(AnimationState.TrackEntryEventDelegate callback);
        void SubscribeComplete(AnimationState.TrackEntryDelegate callback);
        void UnsubcribeEvent(AnimationState.TrackEntryEventDelegate callback);
        void UnsubcribeComplete(AnimationState.TrackEntryDelegate callback);
        bool HasAnimation(string animName);
        bool IsPlaying(string animName);
        bool IsPlaying(int track, string animName);
        void Init(ActorBase actor);
        Spine.Animation FindAnimation(string animName);
        Spine.EventData FindEvent(string eventName);
        Spine.Bone FindBone(string boneName);
        void Play(int track, string animName, bool loop = true, bool restart = false);
        bool EnsurePlay(int track, string animName, bool loop = true, bool restart = false);
        void Pause();
        void Resume();
        void Stop();
        void Clear();
        void ClearTrack(int track);
        public void AddAnimation(int track, string animName, bool loop = false, float delay = 0);
    }
}