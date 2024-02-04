using Cysharp.Threading.Tasks;

public class ActorSetSilentTask : SkillTask
{
    public bool isSilent;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.IsSilent = isSilent;
        IsCompleted = true;
    }
}