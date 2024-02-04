using Cysharp.Threading.Tasks;
using Game.Tasks;
using MoreMountains.Feedbacks;

public class PlayMMFeedbacksTask : Task
{
    public MMF_Player feedbacks;
    public override async UniTask Begin()
    {
        feedbacks.PlayFeedbacks();
        await base.Begin();
        IsCompleted = true;
    }
}