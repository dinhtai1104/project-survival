using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Enemy")]
public class IsInValidZoneDecision : Conditional
{
    public float leftSide, rightSide;
    public SharedActor Actor;
    public override void OnAwake()
    {
        base.OnAwake();
    }
    public override void OnStart()
    {
        base.OnStart();

    }

    public override TaskStatus OnUpdate()
    {
        if (Actor.Value.GetPosition().x<rightSide && Actor.Value.GetPosition().x>leftSide)
        {
            return TaskStatus.Success;

        }
        return TaskStatus.Running;

    }


}
