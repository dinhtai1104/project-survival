using Cysharp.Threading.Tasks;

public class ActorNormalAttackTask : SkillTask
{
    public override async UniTask Begin()
    {
       
        Caster.AttackHandler.Trigger();
        await base.Begin();

        IsCompleted = true;
    }
}
