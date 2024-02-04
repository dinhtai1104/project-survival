using Cysharp.Threading.Tasks;

public class ActorClearTracksTask : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.ClearTracks();
        IsCompleted = true;
    }
}