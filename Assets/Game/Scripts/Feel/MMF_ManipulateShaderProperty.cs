using MoreMountains.Feedbacks;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will modify shader properties")]
[FeedbackPath("Shader/MMF_ManipulateShaderProperty")]
public class MMF_ManipulateShaderProperty : MMF_Feedback
{
    /// a static bool used to disable all feedbacks of this type at once
    public static bool FeedbackTypeAuthorized = true;
    /// sets the inspector color for this feedback
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.EventsColor; } }
    public override bool EvaluateRequiresSetup() { return (Anim == null); }
    public override string RequiredTargetText { get { return Anim.name; } }
    public override string RequiresSetupText { get { return "This feedback requires that you specify a MMGameEventName below."; } }
#endif

    protected Coroutine _coroutine;

    [MMFInspectorGroup("Target Renderer", true, 57, true)]
    public Spine.Unity.SkeletonAnimation Anim;
    public string property;
    public float target,duration;

    public AnimationCurve curve;
    private Material originalMat, newMat;
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
        originalMat = Anim.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
        if (Anim.TryGetComponent<SkeletonRendererCustomMaterials>(out var customMaterial))
        {
            originalMat = Anim.GetComponent<Renderer>().material;
        }
        newMat = new Material(originalMat);


        Anim.CustomMaterialOverride[originalMat] = newMat;
        _coroutine = Owner.StartCoroutine(Lerp(newMat,target,duration,curve));
    }
 
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (newMat != null)
        {
            Anim.CustomMaterialOverride[originalMat] = originalMat;
            Anim.GetComponent<MeshRenderer>().material = originalMat;
            GameObject.Destroy(newMat);
            newMat = null;
        }
    }
    protected virtual IEnumerator Lerp(Material material,float target, float duration, AnimationCurve curve = null)
    {
        if (material == null)
        {
            _coroutine = null;
            IsPlaying = false;

            yield break;
        }
        IsPlaying = true;
        float journey = 0;
        while ((journey >= 0) && (journey <= duration) && (duration > 0))
        {
            float percent = Mathf.Clamp01(journey / duration);

            if (material == null)
            {
                _coroutine = null;
                IsPlaying = false;
                yield break;
            }
            material.SetFloat(property, curve.Evaluate(percent));


            journey +=  FeedbackDeltaTime;
            yield return null;
        }

        if (material != null)
        {
            material.SetFloat(property, target);
        }
        _coroutine = null;
        IsPlaying = false;

        if (newMat != null)
        {
            Anim.CustomMaterialOverride[originalMat] = originalMat;
            Anim.GetComponent<MeshRenderer>().material = originalMat;
            GameObject.Destroy(newMat);
            newMat = null;
        }

        yield break;
    }

    public override void Stop(Vector3 position, float feedbacksIntensity = 1)
    {
        base.Stop(position, feedbacksIntensity);
        if (newMat != null)
        {
            Anim.CustomMaterialOverride[originalMat] = originalMat;
            Anim.GetComponent<MeshRenderer>().material = originalMat;
            GameObject.Destroy(newMat);
            newMat = null;
        }
    }

}
