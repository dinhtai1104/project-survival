using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIDOFade : MonoBehaviour
{
    [SerializeField] private float delay = 0;
    [SerializeField] private float fromAlpha;
    [SerializeField] private float targetAlpha;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private bool isLoop = false;
    [ShowIf(nameof(isLoop), true)] public LoopType loopType = LoopType.Yoyo;
    [SerializeField] private bool customEase;
    [ShowIf(nameof(customEase), true)] public AnimationCurve customCurve;
    private async void OnEnable()
    {
        GetComponent<CanvasGroup>().alpha = fromAlpha;
        await UniTask.Delay((int)(delay * 1000));
        var tween = GetComponent<CanvasGroup>().DOFade(targetAlpha, duration).From(fromAlpha).SetUpdate(true);
        if (isLoop)
        {
            tween.SetLoops(-1, loopType);
        }
        if (customEase)
        {
            tween.SetEase(customCurve);
        }
        else
        {
            tween.SetEase(ease);
        }
        tween.SetId(gameObject);
    }
    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}