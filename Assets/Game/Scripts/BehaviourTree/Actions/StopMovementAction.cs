using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class StopMovementAction : Action
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

    }
    public override TaskStatus OnUpdate()
    {
        actor.MoveHandler.Stop();
        return TaskStatus.Success;
    }
}
