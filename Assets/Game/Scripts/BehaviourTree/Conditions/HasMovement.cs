using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class HasMovement : HasConditional
{
    public override TaskStatus OnUpdate()
    {
        if (!GetComponent<ActorBase>().MoveHandler.Locked)
        {
            return TaskStatus.Success;
        }
        return base.OnUpdate();
    }
}