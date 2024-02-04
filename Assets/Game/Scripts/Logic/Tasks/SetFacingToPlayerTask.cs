using Cysharp.Threading.Tasks;
using Game.Tasks;

public class SetFacingToPlayerTask : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();
        var player = GameController.Instance.GetMainActor();
        if (player != null)
        {
            Caster.SetFacing(player);
        }
        IsCompleted = true;
    }
}
