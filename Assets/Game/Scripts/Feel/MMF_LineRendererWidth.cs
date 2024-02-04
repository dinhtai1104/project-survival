using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will modify shader properties")]
[FeedbackPath("LineRenderer/LineRenderer Width")]
public class MMF_LineRendererWidth : MMF_Feedback
{
    /// a static bool used to disable all feedbacks of this type at once
    public static bool FeedbackTypeAuthorized = true;
    /// sets the inspector color for this feedback
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.EventsColor; } }
    public override bool EvaluateRequiresSetup() { return (lr == null); }
    public override string RequiredTargetText { get { return lr==null?"null":lr.name; } }
    public override string RequiresSetupText { get { return "This feedback requires that you specify a MMGameEventName below."; } }
#endif

    protected Coroutine _coroutine;

    [MMFInspectorGroup("Target Renderer", true, 57, true)]
    public LineRenderer lr;
    public float target, duration;

    public AnimationCurve curve;
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
        _coroutine = Owner.StartCoroutine(Lerp(lr, target, duration, curve));
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    protected virtual IEnumerator Lerp(LineRenderer lr, float target, float duration, AnimationCurve curve = null)
    {
        IsPlaying = true;
        float journey = 0;
        while ((journey >= 0) && (journey <= duration) && (duration > 0))
        {
            float percent = Mathf.Clamp01(journey / duration);

            //material.SetFloat(property, curve.Evaluate(percent));
            lr.widthMultiplier = curve.Evaluate(percent)*target;

            journey += FeedbackDeltaTime;
            yield return null;
        }



        _coroutine = null;
        IsPlaying = false;

    

        yield break;
    }

}
