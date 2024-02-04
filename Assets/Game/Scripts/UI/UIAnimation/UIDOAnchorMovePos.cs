using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class UIDOAnchorMovePos : MonoBehaviour
{
    [SerializeField] private float delay = 0;
    [SerializeField] private Vector3 fromPos;
    [SerializeField] bool IsRandomFromPos;
    [SerializeField, ShowIf(nameof(IsRandomFromPos), true)] private Vector3 fromPos2;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] bool IsRandomTargetPos;
    [SerializeField, ShowIf(nameof(IsRandomTargetPos), true)] private Vector3 toPos2; 
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private bool isLoop = false;
    [ShowIf(nameof(isLoop), true)] public LoopType loopType = LoopType.Yoyo;
    [SerializeField] private bool customEase;
    [ShowIf(nameof(customEase), true)] public AnimationCurve customCurve;

    public UnityEvent callback;
    private Tween tween;
    private Vector3 from;
    private Vector3 to;

    private async void OnEnable()
    {
        from = fromPos;
        to = targetPos;
        if (IsRandomFromPos)
        {
            from = new Vector3(Random.Range(fromPos.x, fromPos2.x), Random.Range(fromPos.y, fromPos2.y));
        }
        if (IsRandomTargetPos)
        {
            to = new Vector3(Random.Range(targetPos.x, toPos2.x), Random.Range(targetPos.y, toPos2.y));
        }

        GetComponent<RectTransform>().anchoredPosition = from;
        await UniTask.Delay((int)(delay * 1000));
        tween = GetComponent<RectTransform>().DOAnchorPos(to, duration).From(from).SetUpdate(true);
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
        tween.OnComplete(() =>
        {
            callback?.Invoke();
        });
        tween.SetId(gameObject);
    }
    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}