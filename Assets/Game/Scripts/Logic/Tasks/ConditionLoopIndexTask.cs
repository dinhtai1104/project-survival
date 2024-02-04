using Cysharp.Threading.Tasks;

public class ConditionLoopIndexTask : SkillTask
{
    public int LoopCount;
    public LoopTask loopTask;
    public override async UniTask Begin()
    {
        await base.Begin();
        if (loopTask.TotalLoop1 == LoopCount)
        {
            loopTask.OnStop();
            loopTask.IsCompleted = true;
        }
        IsCompleted = true;
    }
}