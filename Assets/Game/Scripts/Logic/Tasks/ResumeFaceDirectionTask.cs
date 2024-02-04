using Cysharp.Threading.Tasks;

public class ResumeFaceDirectionTask : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();
        var lastMove = Caster.MoveHandler.lastMove;
        Caster.SetFacing(lastMove.x > 0 ? 1 : -1);
        IsCompleted = true;
    }
}