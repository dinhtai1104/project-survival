using Game.Fsm;
using Game.GameActor;
using UnityEngine;

public class ActorRunToTargetState : BaseState
{
    private float distanceStop = 1f;
    public override void Execute()
    {
        base.Execute();
        if (Actor.IsDead()) return;

        var target = Actor.FindClosetTarget();
        var direction = (target.GetPosition().x - Actor.GetPosition().x);
        if (Mathf.Abs(direction) < distanceStop) return;
        Actor.SetFacing(direction);
        Actor.MoveHandler.Move(new UnityEngine.Vector2(direction, 0), 0);
        Actor.AnimationHandler.SetRun();
    }
}