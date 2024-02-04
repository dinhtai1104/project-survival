using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class FeedbackPlayOnPlayObject : MonoBehaviour
{
    public CharacterObjectBase objectBase;
    public MMF_Player feedback;
    private void OnValidate()
    {
        feedback = GetComponent<MMF_Player>();
    }
    private void OnEnable()
    {
        objectBase.onBeforePlay += BeforePlay;
    }
    private void OnDisable()
    {
        objectBase.onBeforePlay -= BeforePlay;
    }

    private void BeforePlay()
    {
        feedback.PlayFeedbacks();
    }
}