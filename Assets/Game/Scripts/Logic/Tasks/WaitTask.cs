using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;
public class WaitTask : Task
{
    [SerializeField] private float timeRest = 1;

    private float timeCtr = 0;
    public override async UniTask Begin()
    {
        timeCtr = 0;
        await base.Begin();
    }
    public override void Run()
    {
        if (IsCompleted) return;
        base.Run();
        timeCtr += Time.deltaTime;
        if (timeCtr > timeRest)
        {
            IsCompleted = true;
        }
    }
}