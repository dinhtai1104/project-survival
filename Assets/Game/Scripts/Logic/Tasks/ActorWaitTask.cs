using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class ActorWaitTask : SkillTask
{
    private float timeRest;
    public ValueConfigSearch WaitTime;


    private float timeCtr = 0;
    public override async UniTask Begin()
    {
        timeCtr = 0;
        timeRest = WaitTime.SetId(Caster.gameObject.name).FloatValue;
        await base.Begin();
    }
    public override void Run()
    {
        if (IsCompleted || !IsRunning) return;

        base.Run();
        timeCtr += GameTime.Controller.DeltaTime();
        if (timeCtr > timeRest)
        {
            IsCompleted = true;
        }
    }
}
