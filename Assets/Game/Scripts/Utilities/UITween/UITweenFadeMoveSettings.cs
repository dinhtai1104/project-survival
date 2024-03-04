using UnityEngine;

[CreateAssetMenu(menuName = "UI/UITweenFadeMoveSettings")]
public class UITweenFadeMoveSettings : ScriptableObject
{
    public AnimationCurve transitionInBlendCurve;
    public AnimationCurve transitionOutBlendCurve;
    public float transitionInDuration;
    public float transitionOutDuration;
}