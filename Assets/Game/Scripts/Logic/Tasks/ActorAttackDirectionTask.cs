using Cysharp.Threading.Tasks;

public class ActorAttackDirectionTask : SkillTask
{
    public override async UniTask Begin()
    {

        Caster.AttackHandler.Active();
        await base.Begin();
        IsCompleted = true;

    }
}
