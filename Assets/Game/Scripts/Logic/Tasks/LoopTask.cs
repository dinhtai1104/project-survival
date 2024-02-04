using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class LoopTask : SkillTask
{
    public ValueConfigSearch TotalLoop;
    public ValueConfigSearch Delay;
    public List<SkillTask> tasks;
    int currentTaskID;
    SkillTask currentTask => tasks[CurrentTaskID];

    public int CurrentTaskID { get => currentTaskID; set => currentTaskID = value; }
    public int TotalLoop1 { get => totalLoop; set => totalLoop = value; }

    public override async UniTask Begin()
    {


        TotalLoop1 = TotalLoop.SetId(Caster.gameObject.name).IntValue;
        delay = Delay.SetId(Caster.gameObject.name).FloatValue;
        time = 0;
        await base.Begin();
    }
    float time = 0, delay;
    int totalLoop = 0;
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (TotalLoop1 > 0)
        {
            if (time <=0)
            {
                if (!currentTask.IsRunning)
                {
                    currentTask.Begin();
                }
                if (currentTask.IsCompleted)
                {
                    currentTask.End();
                    CurrentTaskID++;
                }
                else
                {
                    currentTask.Run();
                }

                if (CurrentTaskID >= tasks.Count)
                {
                    time = delay;
                    TotalLoop1--;
                    CurrentTaskID = 0;
                }
            }
            else
            {
                time -= GameTime.Controller.DeltaTime();
            }
        }
        else
        {
            IsCompleted = true;
        }

    }
}