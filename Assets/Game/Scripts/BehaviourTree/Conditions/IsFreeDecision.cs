using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Enemy")]
public class IsFreeDecision : Conditional
{
    public SharedBool isChasing;
  

    public override TaskStatus OnUpdate()
    {
        if (!isChasing.Value)
        {
            return TaskStatus.Success;

        }
        return TaskStatus.Running;

    }


}