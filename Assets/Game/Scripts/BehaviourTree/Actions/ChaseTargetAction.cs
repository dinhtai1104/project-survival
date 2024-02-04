using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

public class ChaseTargetAction : Action
{
    [SerializeField] private ActorBase actor;
    [SerializeField] private SharedVector2 destination;


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
        if (actor.Sensor.CurrentTarget == null) return TaskStatus.Running;
        destination.SetValue(new Vector2(actor.Sensor.CurrentTarget.GetPosition().x,actor.GetPosition().y));
        return  TaskStatus.Success ;
    }
}
