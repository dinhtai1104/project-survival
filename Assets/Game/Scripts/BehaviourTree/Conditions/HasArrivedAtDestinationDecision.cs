using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

public class HasArrivedAtDestinationDecision : Conditional
{
    public SharedVector2 destination;
    ActorBase actor;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override TaskStatus OnUpdate()
    {
        return Vector2.Distance(actor.GetPosition(),destination.Value)<1f?TaskStatus.Success:TaskStatus.Running;
    }
}
