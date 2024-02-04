using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

[TaskCategory("Common")]
public class InitAction : Action
{
    [SerializeField] private ActorBase actor;
    [SerializeField] private SharedActor Actor;

    public override void OnStart()
    {
        base.OnStart();
       
    }
    public override TaskStatus OnUpdate()
    {
        if (actor)
        {
            Actor.Value = actor;
            return TaskStatus.Success;
        }
        return base.OnUpdate();
    }
}
