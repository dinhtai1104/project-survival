using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayEffectTask : SkillTask
{
    public ParticleSystem ps;
    public override async UniTask Begin()
    {
        ps.Play();
        await base.Begin();
        IsCompleted = true;
    }

}
