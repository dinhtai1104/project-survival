using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIDOScale : MonoBehaviour
{
    public Vector2 scaleStart;
    public Vector2 scaleEnd;
    public float duration;
    public Ease ease;
    public bool Isloop;
    public float delay;
    private Tween tween;

    private void OnEnable()
    {
        transform.localScale = scaleStart;
        tween = transform.DOScale(scaleEnd, duration).SetEase(ease).SetDelay(delay).SetUpdate(true);
        tween.SetId(gameObject);
        if (Isloop)
        {
            tween.SetLoops(-1, LoopType.Yoyo);
        }
    }
    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}