using Cysharp.Threading.Tasks;
using Spine;
using System;
using UnityEngine;

public class ActorSetAnimationTask : SkillTask
{
    [SerializeField] private string _animation;
    [SerializeField] private int track=0;
    [SerializeField] private bool isLoop;
    [SerializeField] private bool _animationCompleteToCompleteTask = false;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.SetAnimation(track, _animation, isLoop);
        if (_animationCompleteToCompleteTask)
        {
            Caster.AnimationHandler.onCompleteTracking += OnCompleteAnimation;
        }
    }

    private void OnCompleteAnimation(TrackEntry trackEntry)
    {
        if (_animationCompleteToCompleteTask == false) return;
        if (trackEntry.Animation.Name == _animation)
        {
            IsCompleted = true;
        }
    }
    public override UniTask End()
    {
        if (_animationCompleteToCompleteTask)
        {
            Caster.AnimationHandler.onCompleteTracking -= OnCompleteAnimation;
        }
        return base.End();
    }
    public override void Run()
    {
        base.Run();
        if (_animationCompleteToCompleteTask == false)
        {
            IsCompleted = true;
        }
    }
}
