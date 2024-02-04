using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will trigger a MMGameEvent of the specified name when played")]
[FeedbackPath("Events/CustomEvent")]
public class MMF_MMMessengerEvent : MMF_Feedback
{
    /// a static bool used to disable all feedbacks of this type at once
    public static bool FeedbackTypeAuthorized = true;
    /// sets the inspector color for this feedback
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.EventsColor; } }
    public override bool EvaluateRequiresSetup() { return ((int)Key == -1); }
    public override string RequiredTargetText { get { return Key.ToString(); } }
    public override string RequiresSetupText { get { return "This feedback requires that you specify a MMGameEventName below."; } }
#endif

    [MMFInspectorGroup("EventKey", true, 57, true)]
    public EventKey Key;

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
        if (Key == EventKey.StageStart)
        {
            Messenger.Broadcast<Callback>(Key, () => { });
            return;
        }
        Messenger.Broadcast(Key);
    }
}
