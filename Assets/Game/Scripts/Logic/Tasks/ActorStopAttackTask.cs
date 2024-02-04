using Cysharp.Threading.Tasks;

public class ActorStopAttackTask : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AttackHandler.Stop();
        IsCompleted = true;
    }
  
}