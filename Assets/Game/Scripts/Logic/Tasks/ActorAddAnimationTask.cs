using Cysharp.Threading.Tasks;
using Spine;
using UnityEngine;

public class ActorAddAnimationTask : SkillTask
{
    [SerializeField] private string _animation;
    [SerializeField] private int track = 0;
    [SerializeField] private bool isLoop;
    [SerializeField] private bool _animationCompleteToCompleteTask = false;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.AddAnimation(track, _animation, isLoop);
        Caster.AnimationHandler.onCompleteTracking += OnCompleteAnimation;
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
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteAnimation;
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
