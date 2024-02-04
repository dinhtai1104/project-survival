using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ActorTriggerAttackRepeatTask : SkillTask
{
    public ValueConfigSearch FireRate;
    private float fireRate;
    public MMF_Player triggerFb;
    public override async UniTask Begin()
    {
       
        fireRate = FireRate.FloatValue;
        await base.Begin();
    }


    public override UniTask End()
    {
        return base.End();
    }

    float time = 0;
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (Time.time-time>=1f/fireRate)
        {
            triggerFb?.PlayFeedbacks();
            Caster.AttackHandler.Trigger();
            time = Time.time;

        }
       
    }
}