using Cysharp.Threading.Tasks;

public class ActorPlayShootAnimTask : SkillTask
{
    public override async UniTask Begin()
    {
        Caster.AnimationHandler.SetShoot();
        await base.Begin();
        IsCompleted = true;
    }

   
}
