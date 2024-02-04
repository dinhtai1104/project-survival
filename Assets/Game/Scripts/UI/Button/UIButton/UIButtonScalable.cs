using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonScalable : UIBaseButton
{
    public override void OnHoldEvent()
    {
        if (originScale == 0) originScale = 1;
        base.OnHoldEvent();
        transform.DOScale(Vector3.one * 0.9f * originScale, 0.1f).SetUpdate(true);
    }
    public override void OnReleaseEvent()
    {
        if (originScale == 0) originScale = 1;
        base.OnReleaseEvent();
        Sequence sequence = DOTween.Sequence(gameObject).SetUpdate(true);

        sequence.Append(button.transform.DOScale(Vector3.one * 1.2f * originScale, 0.1f))
                .Append(button.transform.DOScale(Vector3.one * 1f * originScale, 0.1f));
    }
    public override void Action()
    {
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        DOTween.Kill(gameObject);
        transform.DOKill();
        transform.localScale = Vector3.one * originScale;
    }
}
