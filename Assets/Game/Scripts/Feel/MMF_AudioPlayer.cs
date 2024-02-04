using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will modify shader properties")]
[FeedbackPath("Audio/MMF_AudioPlayer")]
public class MMF_AudioPlayer : MMF_Feedback
{
    /// a static bool used to disable all feedbacks of this type at once
    public static bool FeedbackTypeAuthorized = true;
    /// sets the inspector color for this feedback
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.EventsColor; } }
    public override bool EvaluateRequiresSetup() { return (clip == null); }
    public override string RequiredTargetText { get { return clip.name; } }
    public override string RequiresSetupText { get { return "This feedback requires that you specify a MMGameEventName below."; } }
#endif

    protected Coroutine _coroutine;

    [MMFInspectorGroup("Target clip", true, 57, true)]
    public AudioClip  clip;
    public float volumn;
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

        FeedbackDuration = clip.length;
        Sound.Controller.Instance.PlayOneShot(clip, volumn);
    }
 
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
 

}
