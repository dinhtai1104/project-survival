using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will modify shader properties")]
[FeedbackPath("Spine/PlayAnimation")]
public class MMF_PlayAnimation : MMF_Feedback
{
    /// a static bool used to disable all feedbacks of this type at once
    public static bool FeedbackTypeAuthorized = true;
    /// sets the inspector color for this feedback
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.EventsColor; } }
    public override bool EvaluateRequiresSetup() { return (anim == null); }
    public override string RequiredTargetText { get { return anim == null ? "null" : anim.name; } }
    public override string RequiresSetupText { get { return "This feedback requires that you specify a MMGameEventName below."; } }
#endif

    protected Coroutine _coroutine;

    [MMFInspectorGroup("Target", true, 57, true)]
    public Spine.Unity.SkeletonAnimation anim;

    public string animationName;
    public bool loop = false;
    protected override void CustomInitialization(MMF_Player owner)
    {
        base.CustomInitialization(owner);

    }
    /// <summary>
    /// On Play we change the values of our fog
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (!Active || !FeedbackTypeAuthorized)
        {
            return;
        }

        FeedbackDuration = 0;

        anim.AnimationState.SetAnimation(0, animationName, loop);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
  

}
