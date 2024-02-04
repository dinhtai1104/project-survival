using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;

public class AchievementQuestNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        return Architecture.Get<AchievementService>().CanReceive();
    }
}
