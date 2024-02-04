using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;

public class PlayFeedBackTask : SkillTask
{
    public MMF_Player player;
    public override async UniTask Begin()
    {
        player.PlayFeedbacks();
        await base.Begin();
        IsCompleted = true;
    }
   
}
