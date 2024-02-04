using Cysharp.Threading.Tasks;

public class ActorStopMovementTask : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.MoveHandler.Stop();
        IsCompleted = true;
    }
   
}


