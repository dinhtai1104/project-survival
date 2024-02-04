using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAction : Action
{
    ActorBase actor;
    public SharedInt defaultDirection;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override void OnStart()
    {
        base.OnStart();
      
    }
    public override TaskStatus OnUpdate()
    {
        if (!actor.MoveHandler.isMoving)
        {
            actor.MoveHandler.Move(actor.MoveHandler.lastMove.magnitude == 0 ? Vector2.right * ( defaultDirection.Value!=0?defaultDirection.Value:(UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1)) : actor.MoveHandler.lastMove, 1);
        }
        return TaskStatus.Success;
    }
}
