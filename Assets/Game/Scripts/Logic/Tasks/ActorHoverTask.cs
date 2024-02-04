using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorHoverTask : SkillTask
{
    [UnityEngine.SerializeField]
    private float hoverTime=1;

    float timer = 0;
    public override async UniTask Begin()
    {
        Caster.GetRigidbody().isKinematic = true;
        Caster.GetRigidbody().velocity = UnityEngine.Vector2.zero;
        timer = 0;

        await base.Begin();
    }
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        timer += Time.deltaTime;
        if (timer>=hoverTime)
        {
            Caster.GetRigidbody().isKinematic = false;
            IsCompleted = true;
        }
    }
}