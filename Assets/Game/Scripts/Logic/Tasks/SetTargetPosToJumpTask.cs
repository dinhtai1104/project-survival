using Cysharp.Threading.Tasks;

public class SetTargetPosToJumpTask : SkillTask
{
    public ActorJumpToTargetTask task;
    public override async UniTask Begin()
    {
        await base.Begin();

        var pos = GameController.Instance.GetMainActor().GetPosition();
        task.SetPos(pos);
        IsCompleted = true;
    }
}