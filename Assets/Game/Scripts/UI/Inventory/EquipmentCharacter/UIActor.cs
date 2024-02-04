using UnityEngine;
using Spine.Unity;

public class UIActor : MonoBehaviour
{
    private ISkeletonAnimation skeletonAnimation;
    private IAnimationStateComponent skeletonState => (IAnimationStateComponent)skeletonAnimation;
    private void Awake()
    {
        skeletonAnimation = GetComponent<ISkeletonAnimation>();
    }
    private void OnEnable()
    {
    }

    public void SetSkin(string nameSkin)
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<ISkeletonAnimation>();
        }
        skeletonAnimation.Skeleton.SetSkin(nameSkin);
    }
    public void SetTimeScale(float timeScale)
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<ISkeletonAnimation>();
        }
        (skeletonAnimation as SkeletonGraphic).timeScale = timeScale;
    }

    public void SetAnimation(int index, string anim, bool isLoop, System.Action<Spine.TrackEntry, Spine.Event> onEvent = null, System.Action<Spine.TrackEntry> onComplete = null)
    {
        var state = skeletonState.AnimationState.SetAnimation(index, anim, isLoop);

        state.Complete += (t)=> onComplete?.Invoke(t);
        state.Event += (t, v)=> onEvent?.Invoke(t, v);
    }
    public void AddAnimation(int index, string anim, bool isLoop, float delay = 0, System.Action<Spine.TrackEntry, Spine.Event> onEvent = null, System.Action<Spine.TrackEntry> onComplete = null)
    {
        var state = skeletonState.AnimationState.AddAnimation(index, anim, isLoop, 0);

        state.Complete += (t) => onComplete?.Invoke(t);
        state.Event += (t, v) => onEvent?.Invoke(t, v);
    }
}