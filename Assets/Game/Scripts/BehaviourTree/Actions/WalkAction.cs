using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

public class WalkAction : Action
{
    ActorBase actor;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override void OnStart()
    {
        base.OnStart();
        actor.MoveHandler.Move(actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right * (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) : actor.MoveHandler.lastMove, 1);
    }
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}