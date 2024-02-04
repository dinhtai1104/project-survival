using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorAddEmptyAnimationTask : SkillTask
{
    [SerializeField] private int track = 0;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.AddEmptyAnimation(track);
        IsCompleted = true;
    }

  
    public override UniTask End()
    {
        return base.End();
    }
 
}
