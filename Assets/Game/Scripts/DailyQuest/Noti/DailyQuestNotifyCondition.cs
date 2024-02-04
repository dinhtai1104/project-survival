using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;

public class DailyQuestNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        return Architecture.Get<QuestService>().CanReceive();
    }
}
