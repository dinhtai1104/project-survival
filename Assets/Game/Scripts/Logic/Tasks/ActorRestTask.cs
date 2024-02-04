using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorRestTask : SkillTask
{
    [SerializeField] private float timeRest = 1;
    private float timeCtr = 0;
    public override async UniTask Begin()
    {
        timeCtr = 0;
        await base.Begin();
        Caster.MoveHandler.Stop();
    }
    public override void Run()
    {
        if (IsCompleted || !IsRunning) return;
        base.Run();
        timeCtr += Time.deltaTime;
        if (timeCtr > timeRest)
        {
            IsCompleted = true;
        }
    }
}
