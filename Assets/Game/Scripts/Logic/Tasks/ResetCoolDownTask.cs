using Cysharp.Threading.Tasks;

public class ResetCoolDownTask : SkillTask
{
    public int SkillId;
    public override async UniTask Begin()
    {
        ((BaseSkill)Caster.SkillEngine.GetSkill(SkillId)).StartCooldown();
        await base.Begin();
        IsCompleted = true;
    }
 
}
