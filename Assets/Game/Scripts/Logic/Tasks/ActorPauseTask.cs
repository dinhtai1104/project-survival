using Cysharp.Threading.Tasks;

public class ActorPauseTask : SkillTask
{
    public bool isPause = true;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.IsActived = !isPause;
        if (isPause)
        {
            Caster.Tagger.AddTag(ETag.Stun);
        }
        else
        {
            Caster.Tagger.RemoveTag(ETag.Stun);
        }
        IsCompleted = true;
    }
}