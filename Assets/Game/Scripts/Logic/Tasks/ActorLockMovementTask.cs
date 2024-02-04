using Cysharp.Threading.Tasks;

public class ActorLockMovementTask : SkillTask
{
    public bool Locked;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.MoveHandler.Locked = Locked;
        if (Locked)
        {
            Caster.MoveHandler.Stop();
        }
        IsCompleted = true;
    }
}