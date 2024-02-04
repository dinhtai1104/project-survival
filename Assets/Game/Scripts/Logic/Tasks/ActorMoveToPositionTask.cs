using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorMoveToPositionTask : SkillTask
{
    public Vector3 destination;
    public override async UniTask Begin()
    {
        await base.Begin();
    }


    public override UniTask End()
    {
        return base.End();
    }
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (Vector3.Distance(Caster.GetPosition(), destination) > 1.5f)
        {
            var direction = (destination - Caster.GetPosition()).normalized;
            Caster.MoveHandler.Move(direction, 1);
        }
        else
        {
            IsCompleted = true;
        }
    }
}