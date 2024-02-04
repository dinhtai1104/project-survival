using com.mec;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UIFadeAndMoveChild : MonoBehaviour
{
    public float DelayMove = 0;
    public float DelayFade = 0;
    public enum SideStart : int { Left = -1, Right = 1 };

    public SideStart sideStart = SideStart.Left;
    public CanvasGroup[] canvasGroups;
    public RectTransform[] rectTrans;
    public float duration = 0.25f;
    public float delayBtw;
    public AnimationCurve curve;

    private void OnEnable()
    {
        Timing.RunCoroutine(_Running(), gameObject);
    }
    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
        DOTween.Kill(gameObject);
    }
    private IEnumerator<float> _Running()
    {
        foreach (var group in canvasGroups)
        {
            group.alpha = 0;
        }
        for (int i = 0; i < rectTrans.Length; i++)
        {
            var rect = rectTrans[i];
            rect.anchoredPosition = Vector2.right * (int)this.sideStart * 300;
        }
        yield return Timing.WaitForSeconds(DelayMove);

        for (int i = 0; i < rectTrans.Length; i++)
        {
            if (rectTrans[i].gameObject.activeSelf == false) continue;
            Sequence sq = DOTween.Sequence(gameObject);
            sq.Join(canvasGroups[i].DOFade(1, duration).SetEase(curve).SetDelay(DelayFade));
            sq.Join(rectTrans[i].DOAnchorPos(Vector3.zero, duration).SetEase(curve));

            yield return Timing.WaitForSeconds(delayBtw);
        }
    }
}