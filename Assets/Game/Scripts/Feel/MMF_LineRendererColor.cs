using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will modify shader properties")]
[FeedbackPath("LineRenderer/LineRenderer Color")]
public class MMF_LineRendererColor : MMF_Feedback
{
    /// a static bool used to disable all feedbacks of this type at once
    public static bool FeedbackTypeAuthorized = true;
    /// sets the inspector color for this feedback
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.EventsColor; } }
    public override bool EvaluateRequiresSetup() { return (lr == null); }
    public override string RequiredTargetText { get { return lr == null ? "null" : lr.name; } }
    public override string RequiresSetupText { get { return "This feedback requires that you specify a MMGameEventName below."; } }
#endif

    protected Coroutine _coroutine;

    [MMFInspectorGroup("Target Renderer", true, 57, true)]
    public LineRenderer lr;
    public float  duration;

    public Gradient gradient;
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

        FeedbackDuration = duration;
        _coroutine = Owner.StartCoroutine(Lerp(lr, duration, gradient));
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    protected virtual IEnumerator Lerp(LineRenderer lr,  float duration, Gradient gradient = null)
    {
        IsPlaying = true;
        float journey = 0;
        while ((journey >= 0) && (journey <= duration) && (duration > 0))
        {
            float percent = Mathf.Clamp01(journey / duration);

            lr.startColor=lr.endColor = gradient.Evaluate(percent);

            journey += FeedbackDeltaTime;
            yield return null;
        }



        _coroutine = null;
        IsPlaying = false;



        yield break;
    }

}
