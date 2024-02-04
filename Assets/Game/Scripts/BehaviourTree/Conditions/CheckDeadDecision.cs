using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class CheckDeadDecision : Conditional
{
    ActorBase actor;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }

    public override TaskStatus OnUpdate()
    {
        return actor.IsDead() ? TaskStatus.Success : TaskStatus.Running;
    }

}