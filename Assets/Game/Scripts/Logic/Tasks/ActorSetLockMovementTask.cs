using Cysharp.Threading.Tasks;

public class ActorSetLockMovementTask : SkillTask
{
    public bool _setLock = false;
    public override async UniTask Begin()
    {
        await base.Begin();
    }
}