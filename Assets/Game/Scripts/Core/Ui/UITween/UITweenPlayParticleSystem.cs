using Cysharp.Threading.Tasks;
using UnityEngine;

public class UITweenPlayParticleSystem : UITweenBase
{
    public ParticleSystem eff;
    public async override UniTask Hide()
    {
        eff.Stop();
        await UniTask.Yield();
    }

    public override async UniTask Show()
    {
        eff.Play();
        await UniTask.Yield();
    }
}
