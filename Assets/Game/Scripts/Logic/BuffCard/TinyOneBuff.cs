using DG.Tweening;
using Game.GameActor.Buff;
using UnityEngine;

public class TinyOneBuff : AbstractBuff
{
    public override void Play()
    {
        Caster.transform.DOScale(Vector3.one * GetValue(StatKey.Size), 0.5f).SetEase(Ease.InBack);
    }
}
