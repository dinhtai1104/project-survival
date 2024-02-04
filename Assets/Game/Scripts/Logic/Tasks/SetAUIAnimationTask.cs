using Cysharp.Threading.Tasks;
using Game.Tasks;
using Spine.Unity;

public class SetAUIAnimationTask : Task
{
    public SkeletonGraphic skeleton;
    [SpineAnimation(dataField:"skeleton")]
    public string _animation;
    public bool isLoop;
    public override async UniTask Begin()
    {
        await base.Begin();
        skeleton.AnimationState.SetAnimation(0, _animation, isLoop);
        IsCompleted = true;
    }
}