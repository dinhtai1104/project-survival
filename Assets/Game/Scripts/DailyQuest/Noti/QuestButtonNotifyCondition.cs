using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;

public class QuestButtonNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        return Architecture.Get<QuestService>().CanReceive() || Architecture.Get<AchievementService>().CanReceive();
    }
}